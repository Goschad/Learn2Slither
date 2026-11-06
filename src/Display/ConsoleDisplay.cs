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
        for (int y = 0; y < board.Size; y++)
        {
            Console.Write(WallChar);
            for (int x = 0; x < board.Size; x++)
                PrintCell(board.GetCell(x, y));
            Console.WriteLine(WallChar);
        }
        PrintWallLine(board.Size);
    }

    public static void PrintSnakeVisual(Board board)
    {
        (int x, int y) = board.SnakeHead;
        Console.WriteLine();

        for (int j = -1; j <= board.Size; j++)
        {
            for (int i = -1; i <= board.Size; i++)
            {
                if (i < 0 || i >= board.Size || j < 0 || j >= board.Size)
                {
                    if (i == x || j == y)
                        Console.Write(WallChar);
                    else
                        PrintNoVision();
                }
                else if (x == i || y == j)
                    PrintCell(board.GetCell(i, j));
                else
                    PrintNoVision();
            }
            Console.WriteLine();
        }
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

    static void PrintNoVision()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write('X');
        Console.ResetColor();
    }
}