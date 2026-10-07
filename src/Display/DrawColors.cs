// ============================================================
// DrawColors.cs
// Display — Color palette used by every drawing file.
// ============================================================

using SFML.Graphics;

namespace Learn2Slither.Display;

public partial class GameWindow
{
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
}
