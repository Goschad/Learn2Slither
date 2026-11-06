
// ============================================================
// DrawText.cs
// Display — Text helpers (left-aligned and centered).
// ============================================================

using SFML.Graphics;
using SFML.System;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    void DrawText(string content, float x, float y, uint size, Color color)
    {
        if (_font == null)
            return;

        using var text = new Text(content, _font)
        {
            CharacterSize = size,
            FillColor = color,
            Position = new Vector2f(x, y)
        };

        _window.Draw(text);
    }

    void DrawTextCentered(string content, float centerX, float y, uint size, Color color)
    {
        if (_font == null)
            return;

        using var text = new Text(content, _font)
        {
            CharacterSize = size,
            FillColor = color
        };

        var bounds = text.GetLocalBounds();

        text.Position = new Vector2f(
            centerX - bounds.Left - bounds.Width / 2f,
            y
        );

        _window.Draw(text);
    }
}
