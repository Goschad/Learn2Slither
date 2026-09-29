namespace Learn2Slither.Core;

public partial class Board
{
    const int InitialSnakeLength = 3;
    const int GreenAppleCount = 2;

    public void Initialize()
    {
        // 1. placer le serpent
        ClearGrid();
        _greenApples.Clear();
        IsGameOver = false;
        Steps = 0;

        PlaceSnake();

        for (int i = 0; i < GreenAppleCount; i++)
            PlaceGreenApple();
        PlaceRedApple();
    }

    private void PlaceSnake()
    {
        Direction[] directions = Enum.GetValues<Direction>();
        Direction direction = directions[_random.Next(directions.Length)];
 
        // The body extends behind the head, opposite to the movement
        var (backX, backY) = direction.Opposite().ToOffset();
        int reach = InitialSnakeLength - 1;
 
        // Pick a head position where the whole body fits inside the board
        int headX, headY;
        do
        {
            headX = _random.Next(Size);
            headY = _random.Next(Size);
        }
        while (!IsInside(headX + backX * reach, headY + backY * reach));
 
        var body = new List<(int x, int y)>();
        for (int i = 0; i < InitialSnakeLength; i++)
            body.Add((headX + backX * i, headY + backY * i));
 
        _snake.Reset(body, direction);
 
        // Write the snake into the grid right away
        _grid[headX, headY] = Cell.SnakeHead;
        for (int i = 1; i < body.Count; i++)
            _grid[body[i].x, body[i].y] = Cell.SnakeBody;
    }
 
    private void PlaceGreenApple()
    {
        if (!TryGetRandomEmptyCell(out var cell))
            return;
        _greenApples.Add(cell);
        _grid[cell.x, cell.y] = Cell.GreenApple;
    }
 
    private void PlaceRedApple()
    {
        if (!TryGetRandomEmptyCell(out var cell))
            return;
        RedApple = cell;
        _grid[cell.x, cell.y] = Cell.RedApple;
    }
}