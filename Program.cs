using System.Diagnostics;
using Learn2Slither.Core;
using Learn2Slither.Audio;
using Learn2Slither.Display;
using static Learn2Slither.Option;

class Program
{
    /// <summary>Time between two snake moves in -human mode (lower = faster).</summary>
    const int HumanStepMs = 150;

    static void Main(string[] args)
    {
        /* Options */
        int sessions = 1;
        int size = 10;
        string? savePath = null;
        string? loadPath = null;
        bool sans = false;
        bool human = false;
        bool visualOn = true;
        bool dontLearn = false;
        bool stepByStep = false;

        /*Options options = args.Length == 0
            ? ShowMenu()
            : */ParseArgs(args, ref sessions, ref size, ref visualOn,
            ref savePath, ref loadPath, ref dontLearn, ref stepByStep, ref human, ref sans);

        if (human)
        {
            if (!visualOn)
            {
                Console.Error.WriteLine("Error: -human needs the window, remove '-visual off'.");
                return;
            }
            RunHuman(sessions, size, sans);
        }
        else
        {
            RunDefault(size, visualOn, sans);
        }
    }

    #region Human mode

    /// <summary>Plays 'sessions' games with the keyboard. No agent, no learning.</summary>
    static void RunHuman(int sessions, int size, bool sans)
    {
        var board = NewBoard(size);
        var window = new GameWindow(board, sans);
        using var audio = new AudioPlayer();

        window.EnableHumanInput();

        for (int session = 1; session <= sessions && window.IsOpen; session++)
        {
            if (session > 1)
            {
                board = NewBoard(size);
                window.SetBoard(board);
            }

            window.SetHudInfo(session, sessions, 0f, isLearning: false);
            window.ClearHumanInput();

            PlayHumanSession(board, window, audio);

            if (!window.IsOpen)
                break; // window closed during the game: stop quietly

            window.AddSessionLength(board.SnakeLength);
            Console.WriteLine($"Game over, length = {board.SnakeLength}, duration = {board.Steps}");
            window.ShowEndOfSession();
        }

        window.Close();
    }

    /// <summary>
    /// One game. The window is read and drawn every frame (60 fps),
    /// but the snake only moves every HumanStepMs milliseconds.
    /// </summary>
    static void PlayHumanSession(Board board, GameWindow window, AudioPlayer audio)
    {
        var clock = Stopwatch.StartNew();

        audio.PlayMusic();
        while (!board.IsGameOver && window.IsOpen)
        {
            window.PollEvents();

            if (clock.ElapsedMilliseconds >= HumanStepMs)
            {
                clock.Restart();
                Direction direction = window.GetHumanDirection(board.CurrentDirection);
                board.Step(direction, audio); // ADAPT: the Board method that moves the snake
            }

            window.Render();
        }
        if (board.IsGameOver && window.IsOpen)
        {
            audio.StopMusic();
            audio.PlayGameOver();
        }
    }

    #endregion

    #region Default mode

    /// <summary>
    /// Current behavior without -human: shows the board.
    /// The agent training loop will go here later.
    /// </summary>
    static void RunDefault(int size, bool visualOn, bool sans)
    {
        var board = NewBoard(size);
        ConsoleDisplay.Print(board);

        if (!visualOn)
            return;

        var window = new GameWindow(board, sans);
        while (window.IsOpen)
        {
            window.PollEvents();
            window.Render();
        }
    }

    #endregion

    static Board NewBoard(int size)
    {
        var board = new Board(size);
        board.Initialize();
        return board;
    }
}