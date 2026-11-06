using Learn2Slither.Core;

namespace Learn2Slither.Agent;

public partial class Agent
{
    private QTable _qTable;
    private Random _rng = new Random();

    private float _epsilon;
    private float _epsilonMin;
    private float _epsilonDecay;
    private float _learningRate;
    private float _gamma;

    public bool Learning { get; set; } = true;
    public float Epsilon => _epsilon;

    #region File

    public void Save(string path) => _qTable.Save(path, _epsilon);

    public void Load(string path)
    {
        _epsilon = _qTable.Load(path);
        Console.WriteLine($"Epsilon loaded: {_epsilon}");
        Console.WriteLine($"Q-table entries: {_qTable.Count}");
    }

    #endregion

    #region Agent

    public Agent(float epsilon = 1.0f, float epsilonMin = 0.001f,
                float epsilonDecay = 0.995f, float lr = 0.1f, float gamma = 0.99f)
    {
        _epsilon = epsilon;
        _epsilonMin = epsilonMin;
        _epsilonDecay = epsilonDecay;
        _learningRate = lr;
        _gamma = gamma;
        _qTable = new QTable();
    }

    public Direction ChooseAction(string state)
    {
        if (_rng.NextDouble() < _epsilon)
            return (Direction)_rng.Next(4);

        return _qTable.GetBestAction(state);
    }

    public void Update(string state, Direction action, float reward, string nextState, bool isDone)
    {
        if (!Learning)
            return;
        float maxNext = isDone ? 0f : _qTable.GetMaxValue(nextState);
        _qTable.Update(state, action, reward, maxNext, _learningRate, _gamma);
    }

    public void DecayEpsilon()
    {
        if (!Learning)
            return;
        _epsilon = Math.Max(_epsilonMin, _epsilon * _epsilonDecay);
    }

    public float GetEpsilon() => _epsilon;

    #endregion
}