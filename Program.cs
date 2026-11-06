using System.Diagnostics;
using Learn2Slither.Core;
using Learn2Slither.Audio;
using Learn2Slither.Display;
using Learn2Slither.Interpreter;
using Learn2Slither.Agent;
using static Learn2Slither.Option;

class Program
{
    static void Main(string[] args)
    {
        /* Options */
        int sessions = 1;
        int size = 10;
        int speed = 150;
        float epsilonDecay = 0.995f;
        string? savePath = null;
        string? loadPath = null;
        bool sans = false;
        bool human = false;
        bool visualOn = true;
        bool dontLearn = false;
        bool stepByStep = false;
        bool stateVision = false;

        if (args.Length == (0))
        {
            using var audio = new AudioPlayer();
            var options = DrawLobby.ShowMenu(audio);

            if (!options.Play)
                return;

            sessions = options.Sessions;
            size = options.Size;
            speed = options.Speed;
            epsilonDecay = options.EpsilonDecay;

            visualOn = options.Visual;
            dontLearn = options.DontLearn;
            human = options.Human;
            sans = options.Sans;
            stepByStep = options.StepByStep;
            stateVision = options.StateVision;

            loadPath = options.LoadPath;
            savePath = options.SavePath;
        }
        else
        {
            ParseArgs(args, ref sessions, ref size, ref visualOn,
                ref savePath, ref loadPath, ref dontLearn,
                ref stepByStep, ref human, ref sans,
                ref stateVision, ref speed, ref epsilonDecay);
        }

        if (human)
        {
            if (!visualOn)
            {
                Console.Error.WriteLine("Error: -human needs the window, remove '-visual off'.");
                return;
            }
            if (speed < 150)
            {
                Console.Error.WriteLine("Error: -speed fast is too fast for a human player, use 'slow' or 'medium'.");
                return;
            }
            if (RejectWithHuman(stepByStep, "-step-by-step")
                || RejectWithHuman(stateVision, "-state-vision")
                || RejectWithHuman(dontLearn, "-dontlearn")
                || RejectWithHuman(loadPath != null, "-load")
                || RejectWithHuman(savePath != null, "-save"))
                return;
            RunHuman(sessions, size, sans, speed);
        }
        else
        {
            if (dontLearn && savePath != null)
            {
                Console.Error.WriteLine("Error: -dontlearn and -save cannot be used together, nothing is learned.");
                return;
            }
            if (!visualOn && stepByStep)
            {
                Console.Error.WriteLine("Error: -step-by-step needs the window, remove '-visual off'.");
                return;
            }
            if (!visualOn && stateVision)
            {
                Console.Error.WriteLine("Error: -state-vision prints in the terminal, but '-visual off' disables all output.");
                return;
            }
            
            var agent = new Agent(epsilonDecay: epsilonDecay);
            if (dontLearn) agent.Learning = false;

            if (loadPath != null)
            {
                if (!File.Exists(loadPath))
                {
                    Console.Error.WriteLine($"Error: model file '{loadPath}' not found");
                    return;
                }
                agent.Load(loadPath);
            }
            RunDefault(sessions, size, visualOn, sans, stepByStep, agent, stateVision, speed, savePath);
        }
    }

    #region Human mode

    static bool RejectWithHuman(bool isSet, string option)
    {
        if (!isSet)
            return false;

        Console.Error.WriteLine($"Error: {option} only works with the AI snake, remove '-human'.");
        return true;
    }

    static void RunHuman(int sessions, int size, bool sans, int speed)
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

            PlayHumanSession(board, window, audio, speed);

            if (!window.IsOpen)
                break;

            window.AddSessionLength(board.SnakeLength);
            if (board.IsGameOver)
            {
                Console.WriteLine($"Game over, max length = {board.MaxLength}, max duration = {board.Steps}");
                window.ShowEndOfSession(false, false);
            }
            else if (board.IsWin)
                window.ShowEndOfSession(false, true);
        }
        window.Close();
    }

    static void PlayHumanSession(Board board, GameWindow window, AudioPlayer audio, int speed)
    {
        var clock = Stopwatch.StartNew();

        audio.PlayMusic();
        while (!board.IsGameOver && !board.IsWin && window.IsOpen)
        {
            window.PollEvents();

            if (clock.ElapsedMilliseconds >= speed)
            {
                clock.Restart();
                Direction direction = window.GetHumanDirection(board.CurrentDirection);
                board.Step(direction, audio);
            }

            window.Render(false);
        }
        if (board.IsGameOver && window.IsOpen)
        {
            audio.StopMusic();
            audio.PlayGameOver();
        }
        else if (board.IsWin && window.IsOpen)
        {
            audio.StopMusic();
            audio.PlayWin();
        }
    }

    #endregion

    #region Default mode

    static void RunDefault(int sessions, int size, bool visualOn, bool sans, bool stepByStep, Agent agent, bool stateVision, int speed, string? savePath)
    {
        float totalLength = 0;
        var board = NewBoard(size);

        if (!visualOn)
        {
            for (int session = 1; session <= sessions; session++)
            {
                if (session > 1)
                    board = NewBoard(size);

                PlayAgentSession(board, null, null, stepByStep, agent, stateVision, speed);
                agent.DecayEpsilon();
                if (board.IsGameOver && savePath == null)
                    Console.WriteLine($"Game over, max length = {board.MaxLength}, max duration = {board.Steps}");

                totalLength += board.SnakeLength;

                if (session % 100 == 0)
                {
                    float averageLength = totalLength / 100;

                    Console.WriteLine($"Session {session} — avg length (last 100):{averageLength:F2}");
                    totalLength = 0;
                }
            }
            
            if (!string.IsNullOrWhiteSpace(savePath))
            {
                string? directory = Path.GetDirectoryName(savePath);

                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                agent.Save(savePath);
                Console.WriteLine($"Saved learning state to {savePath}");
            }
        }
        else
        {
            var window = new GameWindow(board, sans);
            using var audio = new AudioPlayer();

            for (int session = 1; session <= sessions && window.IsOpen; session++)
            {
                if (session > 1)
                {
                    board = NewBoard(size);
                    window.SetBoard(board);
                }

                window.SetHudInfo(session, sessions, 0.01f, isLearning: false);
                PlayAgentSession(board, window, audio, stepByStep, agent, stateVision, speed);
                agent.DecayEpsilon();

                if (!window.IsOpen)
                    break;

                window.AddSessionLength(board.SnakeLength);
                if (board.IsGameOver)
                {
                    Console.WriteLine($"Game over, max length = {board.MaxLength}, max duration = {board.Steps}");
                    window.ShowEndOfSession(false, false);
                }
                else if (board.IsWin)
                    window.ShowEndOfSession(false, true);
            }

            if (!string.IsNullOrWhiteSpace(savePath))
            {
                string? directory = Path.GetDirectoryName(savePath);

                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                agent.Save(savePath);
                Console.WriteLine($"Saved learning state to {savePath}");
            }
            window.Close();
        }
    }

    static void PlayAgentSession(Board board, GameWindow? window, AudioPlayer? audio, bool stepByStep, Agent agent, bool stateVision, int speed)
    {
        var stateBuilder = new StateBuilder(board);

        if (window != null)
        {
            var clock = Stopwatch.StartNew();
            window.EnableStepInput();

            audio?.PlayMusic();
            while (!board.IsGameOver && !board.IsWin && window.IsOpen)
            {
                window.PollEvents();

                bool timeToPlay = stepByStep
                    ? window.ConsumeStepRequest()
                    : clock.ElapsedMilliseconds >= speed;

                if (timeToPlay)
                {
                    clock.Restart();
                    var state = stateBuilder.GetCompactState();

                    Direction action = agent.ChooseAction(state);
                    
                    ConsoleDisplay.PrintSnakeVisual(board);
                    Console.WriteLine($"State: {stateBuilder.GetState()} → Action: {action}");
                    Console.WriteLine($"CompactState: {stateBuilder.GetCompactState()} → Action: {action}");

                    board.Step(action, audio);

                    string nextState = board.IsGameOver ? "" : stateBuilder.GetCompactState();
                    agent.Update(state, board.CurrentDirection, board.GetLastReward(), nextState, board.IsGameOver);
                }

                window.Render(stateVision);
            }
            if (board.IsGameOver && window.IsOpen)
            {
                audio?.StopMusic();
                audio?.PlayGameOver();
            }
            else if (board.IsWin && window.IsOpen)
            {
                audio?.StopMusic();
                audio?.PlayWin();
            }
        }
        else
        {
            while (!board.IsGameOver && !board.IsWin)
            {
                var state = stateBuilder.GetCompactState();
                if (stateVision)
                    ConsoleDisplay.PrintSnakeVisual(board);
                Direction action = agent.ChooseAction(state);
                board.Step(action, null);

                string nextState = board.IsGameOver ? "" : stateBuilder.GetCompactState();
                agent.Update(state, action, board.GetLastReward(), nextState, board.IsGameOver);
            }
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