// ============================================================
// GameTypes.cs
// Game — Shared enums: cell content, directions and move outcomes.
// ============================================================

namespace Learn2Slither.Core;

public enum Cell { Empty, SnakeHead, SnakeBody, GreenApple, RedApple }
 
public enum Direction { Up, Down, Left, Right }

public enum MoveResult
{
    Nothing,
    AteGreenApple,
    AteRedApple,
    HitWall,
    HitSelf
}