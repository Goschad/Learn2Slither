// ============================================================
// Move.cs
// Game — One step of the game: move the snake, eat apples,
// detect death, update counters and the grid.
// ============================================================

namespace Learn2Slither.Core;
using Learn2Slither.Audio;

public partial class Board
{
    int _maxLength;

    /// <summary>Longest length reached during this session.</summary>
    public int MaxLength => Math.Max(_maxLength, SnakeLength);

    /// <summary>Plays one move in the given direction. Does nothing once the game is over.</summary>
    public void Step(Direction direction, AudioPlayer audio)
    {
        if (IsGameOver)
            return;

        Steps++;
        _snake.Direction = direction;

        var (dx, dy) = DirectionOffset(direction);
        var head = _snake.Head;
        (int x, int y) newHead = (head.x + dx, head.y + dy);

        // 1. Death checks, BEFORE moving anything
        if (!IsInside(newHead.x, newHead.y) || HitsOwnBody(newHead))
        {
            IsGameOver = true;
            return;
        }

        // 2. Move according to what is on the target cell
        Cell target = _grid[newHead.x, newHead.y];
        switch (target)
        {
            case Cell.GreenApple:
                _snake.Move(newHead, grow: true);   // +1
                _greenApples.Remove(newHead);
                break;

            case Cell.RedApple:
                _snake.Move(newHead, grow: false);  // same length...
                _snake.RemoveTail();                // ...then -1
                break;

            default:
                _snake.Move(newHead, grow: false);  // same length
                break;
        }

        // 3. The apple (if any) has been eaten
        _grid[newHead.x, newHead.y] = Cell.Empty;
        RefreshSnakeOnGrid();
        _maxLength = Math.Max(_maxLength, SnakeLength);

        // 4. Red apple with length 1 -> length 0 -> dead
        if (SnakeLength == 0)
        {
            IsGameOver = true;
            return;
        }

        // 5. Put a new apple somewhere else
        if (target == Cell.GreenApple)
        {
            audio.PlayGreenApple();
            RespawnGreenApple();
        }
        else if (target == Cell.RedApple)
        {
            audio.PlayRedApple();
            RespawnRedApple();
        }
    }

    #region Helpers

    /// <summary>Screen convention: x to the right, y downward.</summary>
    static (int dx, int dy) DirectionOffset(Direction direction) => direction switch
    {
        Direction.Up    => (0, -1),
        Direction.Down  => (0, 1),
        Direction.Left  => (-1, 0),
        Direction.Right => (1, 0),
        _ => (0, 0)
    };

    /// <summary>
    /// True if the new head lands on the snake's own body.
    /// Exception: the tail cell is allowed, because the tail leaves it during
    /// this same move. Not for length 2, where that would be a U-turn.
    /// </summary>
    bool HitsOwnBody((int x, int y) newHead)
    {
        if (!_snake.Occupies(newHead))
            return false;

        bool isTail = newHead == _snake.Tail;
        return !(isTail && SnakeLength > 2);
    }

    /// <summary>Erases every snake cell from the grid, then redraws the snake from its body.</summary>
    void RefreshSnakeOnGrid()
    {
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                if (_grid[x, y] is Cell.SnakeHead or Cell.SnakeBody)
                    _grid[x, y] = Cell.Empty;

        for (int i = 0; i < _snake.Body.Count; i++)
        {
            var (x, y) = _snake.Body[i];
            _grid[x, y] = i == 0 ? Cell.SnakeHead : Cell.SnakeBody;
        }
    }

    void RespawnGreenApple()
    {
        if (FindFreeCell() is not { } cell)
            return; // board full: one green apple less, no crash

        _greenApples.Add(cell);
        _grid[cell.x, cell.y] = Cell.GreenApple;
    }

    void RespawnRedApple()
    {
        if (FindFreeCell() is not { } cell)
        {
            RedApple = (-1, -1); // board full: no red apple for now
            return;
        }

        RedApple = cell;
        _grid[cell.x, cell.y] = Cell.RedApple;
    }

    /// <summary>Random empty cell, or null if the board is full.</summary>
    (int x, int y)? FindFreeCell()
    {
        var free = new List<(int x, int y)>();
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                if (_grid[x, y] == Cell.Empty)
                    free.Add((x, y));

        return free.Count == 0 ? null : free[Random.Shared.Next(free.Count)];
    }

    #endregion
}