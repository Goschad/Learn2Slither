// ============================================================
// DrawLobby.cs
// Display — Start menu with 4 entries: Start, Settings, Load model, Quit.
// Settings and Load model are their own screens; Esc goes back.
// ============================================================

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Learn2Slither.Audio;

namespace Learn2Slither.Display;

public sealed class MenuSettings
{
    public int Sessions { get; set; } = 1;
    public int Size { get; set; } = 10;
    public int Speed { get; set; } = 150;
    public float EpsilonDecay { get; set; } = 0.995f;

    public bool Visual { get; set; } = true;
    public bool DontLearn { get; set; }
    public bool Human { get; set; }
    public bool Sans { get; set; }
    public bool StepByStep { get; set; }
    public bool StateVision { get; set; }

    public string? LoadPath { get; set; }
    public string? SavePath { get; set; }

    public bool Play { get; set; }
}

enum LobbyScreen { Main, Settings, Model }

public static class DrawLobby
{
    const uint Width = 760;
    const uint Height = 720;

    const int MinSize = 5;
    const int MaxSize = 30;

    const int VisibleRows = 11;

    public static MenuSettings ShowMenu(AudioPlayer audio)
    {
        audio.PlayMenuMusic();
        var options = new MenuSettings();

        string[] fontPaths =
        {
            "assets/fonts/Determination.ttf",
            "assets/arial.ttf",
            "/System/Library/Fonts/Supplemental/Arial.ttf"
        };

        string? fontPath = fontPaths.FirstOrDefault(File.Exists);

        if (fontPath == null)
            throw new FileNotFoundException("No font found. Add assets/arial.ttf.");

        using var font = new Font(fontPath);

        using var window = new RenderWindow(new VideoMode(Width, Height), "Learn2Slither");

        window.SetFramerateLimit(60);

        using Texture? titleTexture = LoadTexture("assets/fonts/title.png");

        #region State

        string[] mainLabels = { "START GAME", "SETTINGS", "LOAD MODEL", "SAVE MODEL", "QUIT" };

        string[] settingsLabels =
        {
            "Sessions", "Board Size", "Speed", "Visual", "Dont Learn",
            "Human", "Sans", "Step By Step", "State Vision", "Epsilon Decay", "BACK"
        };
        const int settingsBackRow = 10;

        // Load model screen: "None", every file of ./models, "Type a path...", "BACK"
        List<string> modelFiles = Directory.Exists("models")
            ? Directory.GetFiles("models", "*.txt").OrderBy(f => f).ToList()
            : new List<string>();

        var modelLabels = new List<string> { "None" };
        modelLabels.AddRange(modelFiles.Select(f => Path.GetFileName(f)!));
        modelLabels.Add("Type a path...");
        modelLabels.Add("BACK");
        int typePathRow = modelFiles.Count + 1;

        int[] speeds = { 300, 150, 50 };
        float[] epsilonDecays = { 0.995f, 0.99999f };

        var screen = LobbyScreen.Main;
        int selected = 0;
        bool editingPath = false;
        bool pathForSave = false;
        string pathInput = "";
        string message = "";

        bool editingSessions = false;
        string sessionsInput = "";
        const int MaxSessionDigits = 7;

        #endregion

        #region Navigation

        int RowCount() => screen switch
        {
            LobbyScreen.Main => mainLabels.Length,
            LobbyScreen.Settings => settingsLabels.Length,
            _ => modelLabels.Count
        };

        void GoTo(LobbyScreen target, int row = 0, string text = "")
        {
            screen = target;
            selected = row;
            message = text;
        }

        // Back goes to the main menu, with the cursor on the entry we came from
        void GoBack() => GoTo(LobbyScreen.Main, screen == LobbyScreen.Settings ? 1 : 2);

        #endregion

        #region Settings values

        void ChangeOption(int direction)
        {
            switch (selected)
            {
                case 0:
                    options.Sessions = Math.Max(1, options.Sessions + direction);
                    break;

                case 1:
                    options.Size = Math.Clamp(options.Size + direction, MinSize, MaxSize);
                    break;

                case 2:
                    int index = Array.IndexOf(speeds, options.Speed);
                    if (index == -1)
                        index = 1;
                    index = (index + direction + speeds.Length) % speeds.Length;
                    options.Speed = speeds[index];
                    break;

                case 3: options.Visual = !options.Visual; break;
                case 4: options.DontLearn = !options.DontLearn; break;
                case 5: options.Human = !options.Human; break;
                case 6: options.Sans = !options.Sans; break;
                case 7: options.StepByStep = !options.StepByStep; break;
                case 8: options.StateVision = !options.StateVision; break;
                case 9:
                    int decayIndex = Array.IndexOf(epsilonDecays, options.EpsilonDecay);
                    if (decayIndex == -1)
                        decayIndex = 0;
                    decayIndex = (decayIndex + direction + epsilonDecays.Length) % epsilonDecays.Length;
                    options.EpsilonDecay = epsilonDecays[decayIndex];
                    break;
            }
        }

        string BoolText(bool value) => value ? "ON" : "OFF";

        string SettingsValue(int index) => index switch
        {
            0 => editingSessions ? sessionsInput + "_" : options.Sessions.ToString(),
            1 => $"{options.Size} x {options.Size}",
            2 => options.Speed switch
            {
                300 => "Slow",
                150 => "Normal",
                50  => "Fast",
                _   => "Normal"
            },
            3 => BoolText(options.Visual),
            4 => BoolText(options.DontLearn),
            5 => BoolText(options.Human),
            6 => BoolText(options.Sans),
            7 => BoolText(options.StepByStep),
            8 => BoolText(options.StateVision),
            9 => options.EpsilonDecay switch
            {
                0.99999f => "Low (0.99999)",
                _        => "Normal (0.995)"
            },
            _ => ""
        };

        string ModelValue(int index)
        {
            if (index == 0)
                return options.LoadPath == null ? "current" : "";
            if (index <= modelFiles.Count)
                return options.LoadPath == modelFiles[index - 1] ? "current" : "";
            return "";
        }

        #endregion

        #region Enter key (one handler per screen)

        void EnterMain()
        {
            switch (selected)
            {
                case 0:
                    options.Play = true;
                    audio.StopMenuMusic();
                    window.Close();
                    break;
                case 1:
                    GoTo(LobbyScreen.Settings);
                    break;
                case 2:
                    GoTo(LobbyScreen.Model);
                    break;
                case 3:
                    editingPath = true;
                    pathForSave = true;
                    pathInput = options.SavePath ?? "models/";
                    message = "";
                    break;
                default:
                    audio.StopMenuMusic();
                    window.Close();
                    break;
            }
        }

        void EnterSettings()
        {
            if (selected == settingsBackRow)
            {
                GoBack();
            }
            else if (selected == 0)
            {
                editingSessions = true;
                sessionsInput = options.Sessions.ToString();
                message = "";
            }
            else
            {
                ChangeOption(1);
            }
        }

        void EnterModel()
        {
            if (selected == 0)
            {
                options.LoadPath = null;
                GoTo(LobbyScreen.Main, 2, "No model selected.");
            }
            else if (selected <= modelFiles.Count)
            {
                options.LoadPath = modelFiles[selected - 1];
                GoTo(LobbyScreen.Main, 2, $"Model selected: {Path.GetFileName(options.LoadPath)}");
            }
            else if (selected == typePathRow)
            {
                editingPath = true;
                pathForSave = false;
                pathInput = options.LoadPath ?? "";
                message = "";
            }
            else
            {
                GoBack();
            }
        }

        #endregion

        #region Events

        window.Closed += (_, _) => window.Close();

        window.KeyPressed += (_, e) =>
        {
            if (editingPath)
            {
                if (e.Code == Keyboard.Key.Enter)
                {
                    editingPath = false;

                    if (pathForSave)
                    {
                        string chosen = pathInput.Trim();

                        if (chosen.Length == 0)
                        {
                            options.SavePath = null;
                            GoTo(LobbyScreen.Main, 3, "Saving disabled.");
                        }
                        else if (chosen.StartsWith("-"))
                        {
                            message = "Invalid path.";
                        }
                        else
                        {
                            options.SavePath = chosen;
                            GoTo(LobbyScreen.Main, 3, File.Exists(chosen)
                                ? "Warning: this file already exists and will be overwritten."
                                : $"Model will be saved to {chosen}");
                        }
                    }
                    else if (File.Exists(pathInput))
                    {
                        options.LoadPath = pathInput;
                        GoTo(LobbyScreen.Main, 2, "Model selected successfully.");
                    }
                    else
                    {
                        message = "Model file not found.";
                    }
                }
                else if (e.Code == Keyboard.Key.Escape)
                {
                    editingPath = false;
                }
                else if (e.Code == Keyboard.Key.Backspace && pathInput.Length > 0)
                {
                    pathInput = pathInput[..^1];
                }
                return;
            }

            if (editingSessions)
            {
                if (e.Code == Keyboard.Key.Enter)
                {
                    editingSessions = false;

                    if (int.TryParse(sessionsInput, out int typed) && typed >= 1)
                        options.Sessions = typed;
                    else
                        message = "Sessions must be a number of 1 or more.";
                }
                else if (e.Code == Keyboard.Key.Escape)
                {
                    editingSessions = false;
                }
                else if (e.Code == Keyboard.Key.Backspace && sessionsInput.Length > 0)
                {
                    sessionsInput = sessionsInput[..^1];
                }
                return;
            }

            switch (e.Code)
            {
                case Keyboard.Key.Up:
                    selected = (selected - 1 + RowCount()) % RowCount();
                    break;

                case Keyboard.Key.Down:
                    selected = (selected + 1) % RowCount();
                    break;

                case Keyboard.Key.Left:
                    if (screen == LobbyScreen.Settings && selected < settingsBackRow)
                        ChangeOption(-1);
                    break;

                case Keyboard.Key.Right:
                    if (screen == LobbyScreen.Settings && selected < settingsBackRow)
                        ChangeOption(1);
                    break;

                case Keyboard.Key.Enter:
                    if (screen == LobbyScreen.Main) EnterMain();
                    else if (screen == LobbyScreen.Settings) EnterSettings();
                    else EnterModel();
                    break;

                case Keyboard.Key.Escape:
                    if (screen == LobbyScreen.Main)
                        window.Close();
                    else
                        GoBack();
                    break;
            }
        };

        window.TextEntered += (_, e) =>
        {
            foreach (char c in e.Unicode)
            {
                if (editingSessions)
                {
                    if (char.IsAsciiDigit(c) && sessionsInput.Length < MaxSessionDigits)
                        sessionsInput += c;
                }
                else if (editingPath)
                {
                    if (!char.IsControl(c) && pathInput.Length < 500)
                        pathInput += c;
                }
            }
        };

        #endregion

        #region Drawing helpers

        void DrawText(string value, float x, float y, uint size, Color color)
        {
            using var text = new Text(value, font, size)
            {
                Position = new Vector2f(x, y),
                FillColor = color
            };
            window.Draw(text);
        }

        void DrawTextCentered(string value, float centerX, float y, uint size, Color color)
        {
            using var text = new Text(value, font, size) { FillColor = color };
            var bounds = text.GetLocalBounds();
            text.Position = new Vector2f(centerX - bounds.Left - bounds.Width / 2f, y);
            window.Draw(text);
        }

        Texture? LoadTexture(string relativePath)
        {
            string path = Path.Combine(AppContext.BaseDirectory, relativePath);
            return File.Exists(path) ? new Texture(path) { Smooth = true } : null;
        }

        void DrawCenteredImage(Texture texture, float centerX, float centerY, float maxWidth, float maxHeight)
        {
            float scale = Math.Min(maxWidth / texture.Size.X, maxHeight / texture.Size.Y);

            using var sprite = new Sprite(texture)
            {
                Origin   = new Vector2f(texture.Size.X / 2f, texture.Size.Y / 2f),
                Position = new Vector2f(centerX, centerY),
                Scale    = new Vector2f(scale, scale)
            };
            window.Draw(sprite);
        }

        void DrawBar(float x, float y, float width, float height, Color color)
        {
            using var bar = new RectangleShape(new Vector2f(width, height))
            {
                Position = new Vector2f(x, y),
                FillColor = color
            };
            window.Draw(bar);
        }

        Color BarColor(bool active) => active ? new Color(45, 95, 175) : new Color(30, 35, 52);
        Color ValueColor(bool active) => active ? new Color(255, 230, 120) : new Color(170, 190, 210);

        #endregion

        #region Screens

        void DrawMain()
        {
            if (titleTexture != null)
                DrawCenteredImage(titleTexture, Width / 2f, 90 + 30, 420, 100);
            else
                DrawTextCentered("LEARN2SLITHER", Width / 2f, 90, 44, new Color(70, 150, 255));

            for (int i = 0; i < mainLabels.Length; i++)
            {
                float y = 200 + i * 75;
                bool active = selected == i;

                DrawBar(180, y, 400, 56, BarColor(active));
                DrawTextCentered(mainLabels[i], Width / 2f, y + 14, 24,
                                 active ? new Color(255, 230, 120) : Color.White);
            }

            string model = options.LoadPath == null ? "none" : Path.GetFileName(options.LoadPath);
            DrawTextCentered($"Model: {model}", Width / 2f, 585, 15, new Color(140, 150, 170));
            DrawTextCentered($"Save to: {options.SavePath ?? "none"}", Width / 2f, 610, 15, new Color(140, 150, 170));
        }

        // Screens 2 and 3: a list of rows (scrolls if there are more than VisibleRows)
        void DrawList(string title, IReadOnlyList<string> labels, Func<int, string> value)
        {
            DrawTextCentered(title, Width / 2f, 40, 30, new Color(70, 150, 255));

            int first = Math.Clamp(selected - VisibleRows / 2, 0, Math.Max(0, labels.Count - VisibleRows));
            int last = Math.Min(labels.Count, first + VisibleRows);

            for (int i = first; i < last; i++)
            {
                float y = 115 + (i - first) * 44;
                bool active = selected == i;

                DrawBar(80, y, 600, 38, BarColor(active));
                DrawText(labels[i], 105, y + 7, 17, Color.White);
                DrawText(value(i), 445, y + 7, 17, ValueColor(active));
            }
        }

        void DrawPathEditor()
        {
            DrawBar(40, 285, 680, 125, new Color(20, 45, 85));
            DrawText(pathForSave ? "SAVE MODEL TO  (empty = no saving)" : "MODEL PATH", 65, 300, 21, Color.White);

            string visiblePath = pathInput.Length > 55
                ? "..." + pathInput[^52..]
                : pathInput;

            DrawText(visiblePath + "_", 65, 340, 15, new Color(255, 230, 120));
            DrawText("ENTER: Confirm   ESC: Cancel", 65, 380, 13, Color.White);
        }

        string Hint() => editingSessions
            ? "Type a number   BACKSPACE: Erase   ENTER: Confirm   ESC: Cancel"
            : screen switch
        {
            LobbyScreen.Settings => "UP/DOWN: Select   LEFT/RIGHT: Change   ENTER on Sessions: Type",
            _ => "UP/DOWN: Select   ENTER: Confirm   ESC: " + (screen == LobbyScreen.Main ? "Quit" : "Back")
        };

        #endregion

        while (window.IsOpen)
        {
            window.DispatchEvents();

            if (!window.IsOpen)
                break;

            window.Clear(new Color(15, 18, 30));

            switch (screen)
            {
                case LobbyScreen.Main:
                    DrawMain();
                    break;
                case LobbyScreen.Settings:
                    DrawList("SETTINGS", settingsLabels, SettingsValue);
                    break;
                default:
                    DrawList("LOAD MODEL", modelLabels, ModelValue);
                    break;
            }

            if (editingPath)
                DrawPathEditor();

            if (!string.IsNullOrEmpty(message))
                DrawTextCentered(message, Width / 2f, 651, 15, new Color(255, 210, 100));

            DrawTextCentered(Hint(), Width / 2f, 690, 13, new Color(140, 150, 170));

            window.Display();
        }

        return options;
    }
}