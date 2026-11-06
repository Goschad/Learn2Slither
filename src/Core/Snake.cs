// ============================================================
// Snake.cs
// Game — The snake itself: body, direction, movement.
// Knows nothing about the grid, apples or rewards.
// ============================================================

namespace Learn2Slither.Core;

public class Snake
{
    private readonly List<(int x, int y)> _body = new();

    public Direction Direction { get; set; }
    public IReadOnlyList<(int x, int y)> Body => _body;
    public (int x, int y) Head => _body[0];
    public (int x, int y) Tail => _body[^1];
    public int Length => _body.Count;

    public bool Occupies((int x, int y) position) => _body.Contains(position);

    public void Reset(IEnumerable<(int x, int y)> body, Direction direction)
    {
        _body.Clear();
        _body.AddRange(body);
        Direction = direction;
    }

    public (int x, int y)? Move((int x, int y) newHead, bool grow)
    {
        _body.Insert(0, newHead);
        if (grow)
            return null;
        return RemoveTail();
    }

    public (int x, int y) RemoveTail()
    {
        var tail = _body[^1];
        _body.RemoveAt(_body.Count - 1);
        return tail;
    }
}