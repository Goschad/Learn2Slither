// ============================================================
// DrawGrid.cs
// Display — Checkerboard background only.
// ============================================================

using SFML.Graphics;
using Learn2Slither.Core;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    void DrawStateVision()
    {
        (int x, int y) = _board.SnakeHead;

        for (int j = 0; j < _board.Size; j++)
        {
            for (int i = 0; i < _board.Size; i++)
            {
                if (x != i && y != j)
                {
                    DrawCellRect((i + j) % 2 == 0 ? GridDark : GridLight, CellPosition(i, j));
                    DrawNoVision(_noVisionTexture, GridDark, i, j);
                }
            }
        }
    }

    void DrawNoVision(Texture? texture, Color fallbackColor, int x, int y)
    {
        if (texture != null)
            DrawCellSprite(texture, CellPosition(x, y));
        else
            DrawCellRect(fallbackColor, CellPosition(x, y));
    }
}
