// ============================================================
// ConsoleDisplay.cs
// Display — Text rendering of the board in the terminal.
// Used for testing, and later when -visual is off.
// ============================================================

using Learn2Slither.Core;

namespace Learn2Slither.Display;

public static class ConsoleDisplay
{
    const char WallChar = 'W';

    public static void Print(Board board)
    {
        PrintWallLine(board.Size);

        // y outside, x inside: each printed line is one row of the board
        for (int y = 0; y < board.Size; y++)
        {
            Console.Write(WallChar);
            for (int x = 0; x < board.Size; x++)
                PrintCell(board.GetCell(x, y));
            Console.WriteLine(WallChar);
        }

        PrintWallLine(board.Size);
    }

    static void PrintWallLine(int size)
    {
        Console.WriteLine(new string(WallChar, size + 2));
    }

    static void PrintCell(Cell cell)
    {
        (char symbol, ConsoleColor color) = cell switch
        {
            Cell.SnakeHead  => ('H', ConsoleColor.Blue),
            Cell.SnakeBody  => ('S', ConsoleColor.DarkBlue),
            Cell.GreenApple => ('G', ConsoleColor.Green),
            Cell.RedApple   => ('R', ConsoleColor.Red),
            _               => ('0', ConsoleColor.DarkGray)
        };

        Console.ForegroundColor = color;
        Console.Write(symbol);
        Console.ResetColor();
    }
}