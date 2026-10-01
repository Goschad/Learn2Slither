// ============================================================
// Draw.cs
// Display — Drawing: grid, HUD panel, end-of-session overlay.
// Only reads the board, never modifies it.
// ============================================================

using SFML.Graphics;
using SFML.System;
using Learn2Slither.Core;

/* separer les draw */

namespace Learn2Slither.Display;

public partial class GameWindow
{
    #region Colors
    static readonly Color BackgroundColor = new(20, 20, 20);
    static readonly Color HudColor        = new(12, 12, 18);
    static readonly Color GridDark        = new(30, 33, 45);
    static readonly Color GridLight       = new(36, 40, 54);
    static readonly Color SnakeHeadColor  = new(50, 120, 255);
    static readonly Color SnakeBodyColor  = new(30, 80, 200);
    static readonly Color GreenAppleColor = new(50, 220, 80);
    static readonly Color RedAppleColor   = new(220, 50, 50);
    static readonly Color WatchingColor   = new(220, 180, 50);
    static readonly Color DimTextColor    = new(130, 130, 130);
    #endregion

    #region Textures

    const string RedApplePath   = "assets/apple/red_apple.png";
    const string GreenApplePath = "assets/apple/green_apple.png";

    Texture? _redAppleTexture;
    Texture? _greenAppleTexture;

    void LoadTextures()
    {
        _redAppleTexture   = TryLoadTexture(RedApplePath);
        _greenAppleTexture = TryLoadTexture(GreenApplePath);
    }

    static Texture? TryLoadTexture(string relativePath)
    {
        string path = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"[Display] Texture not found: {path}");
            return null;
        }

        try
        {
            return new Texture(path) { Smooth = true };
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"[Display] Failed to load {path}: {e.Message}");
            return null;
        }
    }

    #endregion

    int GridPixels => _board.Size * CellSize;

    #region Grid

    void DrawGrid()
    {
        var cellShape = new RectangleShape(new Vector2f(CellSize - CellGap, CellSize - CellGap));

        for (int x = 0; x < _board.Size; x++)
        {
            for (int y = 0; y < _board.Size; y++)
            {
                Cell cell = _board.GetCell(x, y);
                var position = new Vector2f(x * CellSize, y * CellSize);

                // 1. Background (checkerboard, snake, or fallback apple color)
                cellShape.Position = position;
                cellShape.FillColor = GetCellColor(cell, x, y);
                _window.Draw(cellShape);

                // 2. Apple image on top, if any
                Texture? texture = GetCellTexture(cell);
                if (texture != null)
                    DrawCellSprite(texture, position);
            }
        }
    }

    Color GetCellColor(Cell cell, int x, int y) => cell switch
    {
        Cell.SnakeHead  => SnakeHeadColor,
        Cell.SnakeBody  => SnakeBodyColor,
        Cell.GreenApple when _greenAppleTexture == null => GreenAppleColor,
        Cell.RedApple   when _redAppleTexture   == null => RedAppleColor,
        _ => (x + y) % 2 == 0 ? GridDark : GridLight
    };

    Texture? GetCellTexture(Cell cell) => cell switch
    {
        Cell.GreenApple => _greenAppleTexture,
        Cell.RedApple   => _redAppleTexture,
        _ => null
    };

    void DrawCellSprite(Texture texture, Vector2f position)
    {
        float side = CellSize - CellGap;
        using var sprite = new Sprite(texture)
        {
            Position = position,
            Scale = new Vector2f(side / texture.Size.X, side / texture.Size.Y)
        };
        _window.Draw(sprite);
    }

    #endregion

    #region HUD

    void DrawHud()
    {
        _window.Draw(new RectangleShape(new Vector2f(HudWidth, GridPixels))
        {
            Position = new Vector2f(GridPixels, 0),
            FillColor = HudColor
        });

        if (_font == null)
            return;

        float x = GridPixels + 14;
        float y = 18;
        const float LineHeight = 28;

        if (_totalSessions > 0)
        {
            DrawText($"Session  {_session}/{_totalSessions}", x, y, 17, Color.White);
            y += LineHeight * 1.5f;
        }

        DrawText($"Length   {_board.SnakeLength}", x, y, 17, Color.White); y += LineHeight;
        DrawText($"Steps    {_board.Steps}",       x, y, 17, Color.White); y += LineHeight;
        DrawText($"Epsilon  {_epsilon:F3}",        x, y, 17, Color.White); y += LineHeight * 1.8f;

        DrawText(_isLearning ? "LEARNING" : "WATCHING", x, y, 20,
                 _isLearning ? GreenAppleColor : WatchingColor);
        y += LineHeight * 2.4f;

        DrawLengthHistory(x, y, LineHeight);
    }

    void DrawLengthHistory(float x, float y, float lineHeight)
    {
        if (_sessionLengths.Count == 0)
            return;

        DrawText("Last sessions", x, y, 14, DimTextColor);
        y += lineHeight;

        var last = _sessionLengths.TakeLast(10).ToList();
        int maxLength = Math.Max(1, last.Max());
        float barWidth = (HudWidth - 24f) / last.Count;
        const float MaxBarHeight = 80f;
        float baseY = y + MaxBarHeight;

        for (int i = 0; i < last.Count; i++)
        {
            float barHeight = MaxBarHeight * last[i] / maxLength;
            _window.Draw(new RectangleShape(new Vector2f(barWidth - 2, barHeight))
            {
                Position = new Vector2f(x + i * barWidth, baseY - barHeight),
                FillColor = SnakeHeadColor
            });
        }
    }

    #endregion

    #region End overlay

    void DrawEndOverlay()
    {
        _window.Draw(new RectangleShape(new Vector2f(GridPixels, GridPixels))
        {
            FillColor = new Color(0, 0, 0, 160)
        });

        float centerX = GridPixels / 2f;
        float centerY = GridPixels / 2f;
        DrawTextCentered("GAME OVER",                      centerX, centerY - 55, 34, RedAppleColor);
        DrawTextCentered($"Length : {_board.SnakeLength}", centerX, centerY,      22, Color.White);
        DrawTextCentered($"Steps  : {_board.Steps}",       centerX, centerY + 34, 22, Color.White);
    }

    #endregion

    #region Text helpers

    void DrawText(string content, float x, float y, uint size, Color color)
    {
        if (_font == null)
            return;
        _window.Draw(new Text(_font, content)
        {
            CharacterSize = size,
            FillColor = color,
            Position = new Vector2f(x, y)
        });
    }

    void DrawTextCentered(string content, float centerX, float y, uint size, Color color)
    {
        if (_font == null)
            return;
        var text = new Text(_font, content) { CharacterSize = size, FillColor = color };
        var bounds = text.GetLocalBounds();
        text.Position = new Vector2f(centerX - bounds.Size.X / 2f, y);
        _window.Draw(text);
    }

    #endregion
}