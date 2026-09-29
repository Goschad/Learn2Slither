// ============================================================
// DirectionExtensions.cs
// Core — Helpers on Direction, usable like methods: dir.Opposite()
// ============================================================

namespace Learn2Slither.Core;

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction direction) => direction switch
    {
        Direction.Up    => Direction.Down,
        Direction.Down  => Direction.Up,
        Direction.Left  => Direction.Right,
        Direction.Right => Direction.Left,
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

    public static bool IsOppositeOf(this Direction direction, Direction other)
        => direction.Opposite() == other;

    public static (int dx, int dy) ToOffset(this Direction direction) => direction switch
    {
        Direction.Up    => (0, -1),
        Direction.Down  => (0, 1),
        Direction.Left  => (-1, 0),
        Direction.Right => (1, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };
}