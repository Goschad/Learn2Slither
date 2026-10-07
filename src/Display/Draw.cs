// ============================================================
// Draw.cs
// Display — Drawing entry point and shared helpers.
// Only reads the board, never modifies it.
// ============================================================

using SFML.Graphics;
using SFML.System;

namespace Learn2Slither.Display;

public partial class GameWindow
{
    int GridPixels => _board.Size * CellSize;
    float CellSide => CellSize - CellGap;

    void DrawBoard()
    {
        DrawGrid();
        DrawApples();
        DrawSnake();
    }

    static Vector2f CellPosition(int x, int y) => new(x * CellSize, y * CellSize);

    void DrawCellRect(Color color, Vector2f position)
    {
        _window.Draw(new RectangleShape(new Vector2f(CellSide, CellSide))
        {
            Position = position,
            FillColor = color
        });
    }

    void DrawCellSprite(Texture texture, Vector2f position)
    {
        using var sprite = new Sprite(texture)
        {
            Position = position,
            Scale = new Vector2f(CellSide / texture.Size.X, CellSide / texture.Size.Y)
        };
        _window.Draw(sprite);
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
        _window.Draw(sprite);
    }

    void DrawCellSpriteRotated(Texture texture, Vector2f position, float degrees)
    {
        float half = CellSide / 2f;
        using var sprite = new Sprite(texture)
        {
            Origin   = new Vector2f(texture.Size.X / 2f, texture.Size.Y / 2f),
            Position = position + new Vector2f(half, half),
            Scale    = new Vector2f(CellSide / texture.Size.X, CellSide / texture.Size.Y),
            Rotation = degrees
        };
        _window.Draw(sprite);
    }
}