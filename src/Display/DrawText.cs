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
        _window.Draw(new Text(_font, content)
        {
            CharacterSize = size,
            FillColor = color,
            Position = new Vector2f(x, y)
        });
    }

    void DrawTextCentered(string content, float centerX, float y, uint size, Color color)
    {
        if (_font == null)
            return;
        var text = new Text(_font, content) { CharacterSize = size, FillColor = color };
        var bounds = text.GetLocalBounds();
        text.Position = new Vector2f(centerX - bounds.Size.X / 2f, y);
        _window.Draw(text);
    }
}
