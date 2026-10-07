// ============================================================
// DrawApples.cs
// Display — Green and red apples (PNG, or flat color as fallback).
// ============================================================

using SFML.Graphics;
using Learn2Slither.Core;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    void DrawApples()
    {
        for (int x = 0; x < _board.Size; x++)
        {
            for (int y = 0; y < _board.Size; y++)
            {
                Cell cell = _board.GetCell(x, y);
                if (cell == Cell.GreenApple)
                    DrawApple(_greenAppleTexture, GreenAppleColor, x, y);
                else if (cell == Cell.RedApple)
                    DrawApple(_redAppleTexture, RedAppleColor, x, y);
            }
        }
    }

    void DrawApple(Texture? texture, Color fallbackColor, int x, int y)
    {
        if (texture != null)
            DrawCellSprite(texture, CellPosition(x, y));
        else
            DrawCellRect(fallbackColor, CellPosition(x, y));
    }
}
