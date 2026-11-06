// ============================================================
// StepInput.cs
// Display — Key press that triggers one move in -step-by-step mode.
// The window keeps running (no blocking Console.ReadLine).
// ============================================================

using SFML.Window;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    bool _stepRequested;

    public void EnableStepInput()
    {
        _window.SetKeyRepeatEnabled(true);

        _window.KeyPressed += (_, e) =>
        {
            if (e.Code is Keyboard.Key.Space or Keyboard.Key.Enter or Keyboard.Key.Right)
                _stepRequested = true;
        };
    }

    public bool ConsumeStepRequest()
    {
        bool requested = _stepRequested;
        _stepRequested = false;
        return requested;
    }
}