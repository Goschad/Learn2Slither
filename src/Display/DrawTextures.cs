// ============================================================
// DrawTextures.cs
// Display — Loading of PNG textures (once, at startup).
// ============================================================

using SFML.Graphics;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    const string RedApplePath   = "assets/apple/red_apple.png";
    const string GreenApplePath = "assets/apple/green_apple.png";

    const string SnakeHeadPath   = "assets/snake/head.png";
    const string SansHeadPath   = "assets/snake/head-s.png";
    const string SnakeBodyPath   = "assets/snake/body.png";
    const string SnakeCornerPath = "assets/snake/corner.png";
    const string SnakeTailPath   = "assets/snake/tail.png";

    const string TitlePath    = "assets/fonts/title.png";
    const string GameOverPath    = "assets/fonts/game-over.png";

    Texture? _redAppleTexture;
    Texture? _greenAppleTexture;

    Texture? _snakeHeadTexture;
    Texture? _snakeBodyTexture;
    Texture? _snakeCornerTexture;
    Texture? _snakeTailTexture;

    Texture? _titleTexture;
    Texture? _gameOverTexture;

    void LoadTextures(bool sans)
    {
        _redAppleTexture   = TryLoadTexture(RedApplePath);
        _greenAppleTexture = TryLoadTexture(GreenApplePath);

        _snakeHeadTexture   = sans ? TryLoadTexture(SansHeadPath) : TryLoadTexture(SnakeHeadPath);
        _snakeBodyTexture   = TryLoadTexture(SnakeBodyPath);
        _snakeCornerTexture = TryLoadTexture(SnakeCornerPath);
        _snakeTailTexture   = TryLoadTexture(SnakeTailPath);

        _titleTexture       = TryLoadTexture(TitlePath);
        _gameOverTexture    = TryLoadTexture(GameOverPath);
        
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
}