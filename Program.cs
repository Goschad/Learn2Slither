using Learn2Slither.Core;
using Learn2Slither.Display;
using static Learn2Slither.Option;

class Program
{
    static void Main(string[] args)
    {
        /* Options */
        int sessions = 1;
        int size = 10;
        string? savePath = null;
        string? loadPath = null;
        bool visualOn = true;
        bool dontLearn = false;
        bool stepByStep = false;

        ParseArgs(args, ref sessions, ref size, ref visualOn, 
        ref savePath, ref loadPath, ref dontLearn, ref stepByStep);   

        /* Game */
        var board = new Board(size);
        board.Initialize();

        ConsoleDisplay.Print(board);

        GameWindow? window = visualOn ? new GameWindow(board) : null;

        if (window == null)
            ConsoleDisplay.Print(board);

        while (window is { IsOpen: true })
        {
            window.PollEvents();
            window.Render();
        }
    }
}
