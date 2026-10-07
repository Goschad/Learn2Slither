// ============================================================
// DrawSnake.cs
// Display — Snake drawn with head / body / corner / tail sprites,
// each rotated according to its neighbors.
// ============================================================

using SFML.Graphics;
using SFML.System;
using Learn2Slither.Core;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    #region Directions

    // Screen coordinates: Y grows downward.
    static readonly Vector2i Up    = new(0, -1);
    static readonly Vector2i Right = new(1, 0);
    static readonly Vector2i Down  = new(0, 1);
    static readonly Vector2i Left  = new(-1, 0);

    #endregion

    #region Asset orientation (ADJUST TO YOUR PNG FILES)

    /// <summary>Direction the head is looking at in head.png.</summary>
    static readonly Vector2i HeadFacing = Down;

    /// <summary>Direction the tip of the tail points to in tail.png.</summary>
    static readonly Vector2i TailPointing = Down;

    /// <summary>Axis of body.png: Up (vertical) or Right (horizontal).</summary>
    static readonly Vector2i BodyAxis = Up;

    /// <summary>The two sides of the cell that corner.png connects.</summary>
    static readonly Vector2i CornerSideA = Down;
    static readonly Vector2i CornerSideB = Left;

    #endregion

    void DrawSnake()
    {
        IReadOnlyList<Vector2i> segments = GetSnakeSegments();
        int last = segments.Count - 1;

        for (int i = 0; i < segments.Count; i++)
        {
            Vector2f position = CellPosition(segments[i].X, segments[i].Y);

            if (i == 0)
                DrawSnakeHead(position);
            else if (i == last)
                DrawSnakeTail(segments[i] - segments[i - 1], position);
            else
                DrawSnakeBody(segments[i - 1] - segments[i], segments[i + 1] - segments[i], position);
        }
    }

    #region Segment drawing

    void DrawSnakeHead(Vector2f position)
    {
        if (_snakeHeadTexture == null)
        {
            DrawCellRect(SnakeHeadColor, position);
            return;
        }
        Vector2i facing = ToVector(_board.CurrentDirection);
        float angle = DirectionAngle(facing) - DirectionAngle(HeadFacing);
        DrawCellSpriteRotated(_snakeHeadTexture, position, angle);
    }

    /// <param name="pointing">Direction from the previous segment to the tail.</param>
    void DrawSnakeTail(Vector2i pointing, Vector2f position)
    {
        if (_snakeTailTexture == null)
        {
            DrawCellRect(SnakeBodyColor, position);
            return;
        }
        float angle = DirectionAngle(pointing) - DirectionAngle(TailPointing);
        DrawCellSpriteRotated(_snakeTailTexture, position, angle);
    }

    /// <param name="toPrev">Direction toward the segment closer to the head.</param>
    /// <param name="toNext">Direction toward the segment closer to the tail.</param>
    void DrawSnakeBody(Vector2i toPrev, Vector2i toNext, Vector2f position)
    {
        bool isStraight = toPrev.X + toNext.X == 0 && toPrev.Y + toNext.Y == 0;

        if (isStraight)
        {
            if (_snakeBodyTexture == null)
            {
                DrawCellRect(SnakeBodyColor, position);
                return;
            }
            // A straight piece is symmetric: 180° off makes no difference.
            float angle = DirectionAngle(toPrev) - DirectionAngle(BodyAxis);
            DrawCellSpriteRotated(_snakeBodyTexture, position, angle);
        }
        else
        {
            if (_snakeCornerTexture == null)
            {
                DrawCellRect(SnakeBodyColor, position);
                return;
            }
            DrawCellSpriteRotated(_snakeCornerTexture, position, CornerAngle(toPrev, toNext));
        }
    }

    #endregion

    #region Direction math

    /// <summary>Clockwise angle of a direction, with Up = 0°.</summary>
    static float DirectionAngle(Vector2i direction) => (direction.X, direction.Y) switch
    {
        (0, -1) => 0f,
        (1, 0)  => 90f,
        (0, 1)  => 180f,
        (-1, 0) => 270f,
        _ => 0f
    };

    static Vector2i ToVector(Direction direction) => direction switch
    {
        Direction.Up    => Up,
        Direction.Down  => Down,
        Direction.Left  => Left,
        Direction.Right => Right,
        _ => Up
    };

    /// <summary>Rotates a direction by 90° clockwise (on screen, Y down).</summary>
    static Vector2i RotateClockwise(Vector2i direction) => new(-direction.Y, direction.X);

    /// <summary>
    /// Finds how many quarter turns map the corner.png sides onto the two
    /// actual neighbor directions. The pair has no order: {A, B} == {B, A}.
    /// </summary>
    static float CornerAngle(Vector2i dirA, Vector2i dirB)
    {
        Vector2i sideA = CornerSideA;
        Vector2i sideB = CornerSideB;

        for (int quarterTurns = 0; quarterTurns < 4; quarterTurns++)
        {
            bool matches = (sideA == dirA && sideB == dirB) || (sideA == dirB && sideB == dirA);
            if (matches)
                return quarterTurns * 90f;

            sideA = RotateClockwise(sideA);
            sideB = RotateClockwise(sideB);
        }
        return 0f;
    }

    #endregion

    #region Board access

    /// <summary>Snake positions in order, from head (index 0) to tail.</summary>
    IReadOnlyList<Vector2i> GetSnakeSegments() =>
        _board.SnakeBody.Select(p => new Vector2i(p.x, p.y)).ToList();

    #endregion
}