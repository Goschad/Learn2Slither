// ============================================================
// HumanInput.cs
// Display — Keyboard input for the -human mode.
// Captures key presses, never touches the board.
// ============================================================

using SFML.Window;
using Learn2Slither.Core;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    readonly Queue<Direction> _humanInputs = new();
    const int MaxBufferedInputs = 2;

    bool _humanInputEnabled;

    #region Public interface

    public void EnableHumanInput()
    {
        if (_humanInputEnabled)
            return;
        _window.KeyPressed += OnKeyPressed;
        _humanInputEnabled = true;
    }

    public void ClearHumanInput() => _humanInputs.Clear();

    public Direction GetHumanDirection(Direction current)
    {
        while (_humanInputs.Count > 0)
        {
            Direction wanted = _humanInputs.Dequeue();
            if (wanted != current && wanted != Opposite(current))
                return wanted;
        }
        return current;
    }

    #endregion

    #region Keyboard

    void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        Direction? direction = e.Code switch
        {
            Keyboard.Key.Up    or Keyboard.Key.W or Keyboard.Key.Z => Direction.Up,
            Keyboard.Key.Down  or Keyboard.Key.S                   => Direction.Down,
            Keyboard.Key.Left  or Keyboard.Key.A or Keyboard.Key.Q => Direction.Left,
            Keyboard.Key.Right or Keyboard.Key.D                   => Direction.Right,
            _ => null
        };

        if (direction is { } d && _humanInputs.Count < MaxBufferedInputs)
            _humanInputs.Enqueue(d);
    }

    static Direction Opposite(Direction direction) => direction switch
    {
        Direction.Up    => Direction.Down,
        Direction.Down  => Direction.Up,
        Direction.Left  => Direction.Right,
        Direction.Right => Direction.Left,
        _ => direction
    };

    #endregion
}