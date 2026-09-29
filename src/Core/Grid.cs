// ============================================================
// Board.Grid.cs
// Core — Grid helpers: clearing and finding free cells.
// ============================================================

namespace Learn2Slither.Core;

public partial class Board
{
    private readonly Random _random = new();

    private void ClearGrid()
    {
        Array.Clear(_grid);
    }

    private bool TryGetRandomEmptyCell(out (int x, int y) cell)
    {
        var emptyCells = new List<(int x, int y)>();

        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                if (_grid[x, y] == Cell.Empty)
                    emptyCells.Add((x, y));

        if (emptyCells.Count == 0)
        {
            cell = default;
            return false;
        }

        cell = emptyCells[_random.Next(emptyCells.Count)];
        return true;
    }
}