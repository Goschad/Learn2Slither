// ============================================================
// Move.cs
// Game — One step of the game: move the snake, eat apples,
// detect death, update counters and the grid.
// ============================================================

using Learn2Slither.Audio;

namespace Learn2Slither.Core;

public partial class Board
{
    int _maxLength;

    public int MaxLength => Math.Max(_maxLength, SnakeLength);

    public void Step(Direction direction, AudioPlayer? audio)
    {
        if (IsGameOver)
            return;
        _snake.Direction = direction;

        IncrementStep();
        _snake.Direction = direction;

        var (dx, dy) = DirectionOffset(direction);
        var head = _snake.Head;
        (int x, int y) newHead = (head.x + dx, head.y + dy);

        if (HitsOwnBody(newHead))
        {
            IsGameOver = true;
            SetLastReward(Rewards.SelfCollision);
            return;
        }
        else if (!IsInside(newHead.x, newHead.y))
        {
            IsGameOver = true;
            SetLastReward(Rewards.GameOver);
            return;
        }

        SetLastReward(Rewards.Nothing);

        Cell target = _grid[newHead.x, newHead.y];
        switch (target)
        {
            case Cell.GreenApple:
                _snake.Move(newHead, grow: true);
                _greenApples.Remove(newHead);
                SetLastReward(Rewards.GreenApple);
                SetScore(Rewards.GreenApple);
                break;

            case Cell.RedApple:
                _snake.Move(newHead, grow: false);
                _snake.RemoveTail();
                SetLastReward(Rewards.RedApple);
                SetScore(Rewards.RedApple);
                break;

            default:
                _snake.Move(newHead, grow: false);
                break;
        }

        _grid[newHead.x, newHead.y] = Cell.Empty;
        RefreshSnakeOnGrid();
        _maxLength = Math.Max(_maxLength, SnakeLength);

        if (SnakeLength == 0)
        {
            IsGameOver = true;
            SetLastReward(Rewards.GameOver);
            return;
        }
        else if (SnakeLength >= (Size * Size) - 1)
        {
            SetWin(true);
            return;
        }

        if (target == Cell.GreenApple)
        {
            if (audio != null) 
                audio.PlayGreenApple();
            RespawnGreenApple();
        }
        else if (target == Cell.RedApple)
        {
            if (audio != null) 
                audio.PlayRedApple();
            RespawnRedApple();
        }
    }

    #region Helpers

    static (int dx, int dy) DirectionOffset(Direction direction) => direction switch
    {
        Direction.Up    => (0, -1),
        Direction.Down  => (0, 1),
        Direction.Left  => (-1, 0),
        Direction.Right => (1, 0),
        _ => (0, 0)
    };

    bool HitsOwnBody((int x, int y) newHead)
    {
        if (!_snake.Occupies(newHead))
            return false;

        bool isTail = newHead == _snake.Tail;
        return !(isTail && SnakeLength > 2);
    }

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
            return;

        _greenApples.Add(cell);
        _grid[cell.x, cell.y] = Cell.GreenApple;
    }

    void RespawnRedApple()
    {
        if (FindFreeCell() is not { } cell)
        {
            RedApple = (-1, -1);
            return;
        }

        RedApple = cell;
        _grid[cell.x, cell.y] = Cell.RedApple;
    }

    (int x, int y)? FindFreeCell()
    {
        var free = new List<(int x, int y)>();
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                if (_grid[x, y] == Cell.Empty)
                    free.Add((x, y));

        return free.Count == 0 ? null : free[Random.Shared.Next(free.Count)];
    }

    public bool IsOppositeDirection(Direction action)
    {
        return (action == Direction.Up && _snake.Direction == Direction.Down) ||
               (action == Direction.Down && _snake.Direction == Direction.Up) ||
               (action == Direction.Left && _snake.Direction == Direction.Right) ||
               (action == Direction.Right && _snake.Direction == Direction.Left);
    }

    #endregion
}