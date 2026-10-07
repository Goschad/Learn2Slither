// ============================================================
// DrawOverlay.cs
// Display — "Game over" screen shown at the end of a session.
// ============================================================

using SFML.Graphics;
using SFML.System;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    void DrawEndOverlay()
    {
        _window.Draw(new RectangleShape(new Vector2f(GridPixels, GridPixels))
        {
            FillColor = new Color(0, 0, 0, 160)
        });

        float centerX = GridPixels / 2f;
        float centerY = GridPixels / 2f;
        if (_gameOverTexture != null)
            DrawCenteredImage(_gameOverTexture, centerX, centerY - 55, GridPixels * 0.7f, GridPixels * 0.25f);
        else
            DrawTextCentered("GAME OVER", centerX, centerY - 55, 34, RedAppleColor);
        DrawTextCentered($"Length : {_board.SnakeLength}", centerX, centerY,      22, Color.White);
        DrawTextCentered($"Steps  : {_board.Steps}",       centerX, centerY + 34, 22, Color.White);
    }
}
