// ============================================================
// DrawHud.cs
// Display — Right-side panel: session info and length history.
// ============================================================

using SFML.Graphics;
using SFML.System;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    void DrawHud()
    {
        _window.Draw(new RectangleShape(new Vector2f(HudWidth, GridPixels))
        {
            Position = new Vector2f(GridPixels, 0),
            FillColor = HudColor
        });

        if (_font == null)
            return;

        float x = GridPixels + 14;
        float y = 18;
        const float LineHeight = 28;

        if (_totalSessions > 0)
        {
            DrawText($"Session  {_session}/{_totalSessions}", x, y, 17, Color.White);
            y += LineHeight * 1.5f;
        }

        const float TitleHeight = 60f;

        if (_titleTexture != null)
        {
            float centerX = GridPixels + HudWidth / 2f;
            DrawCenteredImage(_titleTexture, centerX, y + TitleHeight / 2f, HudWidth - 28, TitleHeight);
            y += TitleHeight + 10;
        }
        else
        {
            DrawText("Learn2Slither", x, y, 17, Color.White);
            y += LineHeight;
        }

        DrawText($"Length   {_board.SnakeLength}", x, y, 17, Color.White); y += LineHeight;
        DrawText($"Steps    {_board.Steps}",       x, y, 17, Color.White); y += LineHeight;
        DrawText($"Epsilon  {_epsilon:F3}",        x, y, 17, Color.White); y += LineHeight * 1.8f;

        DrawText(_isLearning ? "LEARNING" : "WATCHING", x, y, 20,
                 _isLearning ? GreenAppleColor : WatchingColor);
        y += LineHeight * 2.4f;

        DrawLengthHistory(x, y, LineHeight);
    }

    void DrawLengthHistory(float x, float y, float lineHeight)
    {
        if (_sessionLengths.Count == 0)
            return;

        DrawText("Last sessions", x, y, 14, DimTextColor);
        y += lineHeight;

        var last = _sessionLengths.TakeLast(10).ToList();
        int maxLength = Math.Max(1, last.Max());
        float barWidth = (HudWidth - 24f) / last.Count;
        const float MaxBarHeight = 80f;
        float baseY = y + MaxBarHeight;

        for (int i = 0; i < last.Count; i++)
        {
            float barHeight = MaxBarHeight * last[i] / maxLength;
            _window.Draw(new RectangleShape(new Vector2f(barWidth - 2, barHeight))
            {
                Position = new Vector2f(x + i * barWidth, baseY - barHeight),
                FillColor = SnakeHeadColor
            });
        }
    }
}
