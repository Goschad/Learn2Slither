// ============================================================
// DrawGrid.cs
// Display — Checkerboard background only.
// ============================================================

namespace Learn2Slither.Display;

public partial class GameWindow
{
    void DrawGrid()
    {
        for (int x = 0; x < _board.Size; x++)
            for (int y = 0; y < _board.Size; y++)
                DrawCellRect((x + y) % 2 == 0 ? GridDark : GridLight, CellPosition(x, y));
    }
}
