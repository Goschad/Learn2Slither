// ============================================================
// GameWindow.cs
// Display — SFML window: creation, public interface, font loading.
// Drawing code lives in GameWindow.Draw.cs.
// ============================================================

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Learn2Slither.Core;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    const int CellSize = 60;
    const int CellGap = 0;
    const int HudWidth = 240;
    const double EndScreenSeconds = 2.0;

    readonly RenderWindow _window;
    readonly Font? _font;
    Board _board;

    int _session;
    int _totalSessions;
    float _epsilon;
    bool _isLearning;
    readonly List<int> _sessionLengths = new();

    public bool IsOpen => _window.IsOpen;

    public GameWindow(Board board, bool sans)
    {
        _board = board;
        _font = TryLoadFont();
        LoadTextures(sans);

        uint gridPixels = (uint)(board.Size * CellSize);
        var mode = new VideoMode(gridPixels + HudWidth, gridPixels);

        _window = new RenderWindow(mode, "Learn2Slither");
        _window.SetFramerateLimit(60);
        _window.Closed += (_, _) => _window.Close();
    }

    #region Public interface

    public void SetBoard(Board board) => _board = board;

    public void SetHudInfo(int session, int totalSessions, float epsilon, bool isLearning)
    {
        _session = session;
        _totalSessions = totalSessions;
        _epsilon = epsilon;
        _isLearning = isLearning;
    }

    public void AddSessionLength(int length) => _sessionLengths.Add(length);

    public void PollEvents() => _window.DispatchEvents();

    public void Render(bool stateVision)
    {
        _window.Clear(BackgroundColor);
        DrawBoard(stateVision);
        DrawHud();
        _window.Display();
    }

    public void ShowEndOfSession(bool stateVision, bool win)
    {
        var until = DateTime.Now.AddSeconds(EndScreenSeconds);
        while (DateTime.Now < until && _window.IsOpen)
        {
            _window.DispatchEvents();
            _window.Clear(BackgroundColor);
            DrawBoard(stateVision);
            DrawHud();
            if (win)
                DrawWinEndOverlay();
            else
                DrawEndOverlay();
            _window.Display();
        }
    }

    public void Close() => _window.Close();

    #endregion

    #region Font

    static Font? TryLoadFont()
    {
        string[] candidates =
        {
            "assets/fonts/Determination.ttf",
            "/System/Library/Fonts/Supplemental/Arial.ttf",
            "/Library/Fonts/Arial.ttf",
            "/System/Library/Fonts/Geneva.ttf",
            "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
            "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf",
            @"C:\Windows\Fonts\arial.ttf"
        };

        foreach (var path in candidates)
        {
            try
            {
                if (File.Exists(path))
                    return new Font(path);
            }
            catch
            {
                // Unreadable font: try the next one
                // slow can't be use with human
            }
        }
        return null;
    }

    #endregion
}