using Learn2Slither.Core;

namespace Learn2Slither.Agent;

public partial class QTable
{
    #region Table

    private Dictionary<string, float[]> _table = new();
    public int Count => _table.Count;

    public float[] GetQValues(string state)
    {
        if (!_table.TryGetValue(state, out var q))
        {
            q = new float[4];
            _table[state] = q;
        }

        return q;
    }

    public Direction GetBestAction(string state)
    {
        float[] q = GetQValues(state);

        float max = float.NegativeInfinity;
        int bestIndex = 0;
        int ties = 0;

        for (int i = 0; i < q.Length; i++)
        {
            if (q[i] > max)
            {
                max = q[i];
                bestIndex = i;
                ties = 1;
            }
            else if (q[i] == max)
            {
                ties++;

                if (Random.Shared.Next(ties) == 0)
                    bestIndex = i;
            }
        }

        return (Direction)bestIndex;
    }

    public float GetMaxValue(string state)
    {
        float[] q = GetQValues(state);
        float max = q[0];

        for (int i = 1; i < q.Length; i++)
        {
            if (q[i] > max)
                max = q[i];
        }

        return max;
    }

    public void Update(string state, Direction action, float reward, float maxNext, float lr, float gamma)
    {
        int actionIndex = (int)action;
        float[] qValues = GetQValues(state);

        qValues[actionIndex] += lr * (reward + gamma * maxNext - qValues[actionIndex]);
    }

    #endregion

    #region File

    public void Save(string path, float epsilon)
    {
        using var writer = new StreamWriter(path);
        writer.WriteLine($"epsilon:{epsilon.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        foreach (var (state, values) in _table)
        {
            var valStr = string.Join(",", values.Select(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            writer.WriteLine($"{state}|{valStr}");
        }
    }

    public float Load(string path)
    {
        float epsilon = 1.0f;
        if (!File.Exists(path)) return epsilon;
        _table.Clear();
        foreach (var line in File.ReadAllLines(path))
        {
            if (line.StartsWith("epsilon:"))
            {
                epsilon = float.Parse(line.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture);
                continue;
            }
            int lastPipe = line.LastIndexOf('|');
            if (lastPipe == -1) continue;
            var state = line.Substring(0, lastPipe);
            var values = line.Substring(lastPipe + 1)
                .Split(',')
                .Select(v => float.Parse(v, System.Globalization.CultureInfo.InvariantCulture))
                .ToArray();
            _table[state] = values;
        }
        return epsilon;
    }
    
    #endregion
}