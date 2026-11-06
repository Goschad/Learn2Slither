// ============================================================
// Board.cs
// Game — Board state: grid, snake, apples. Read-only public interface.
// ============================================================

namespace Learn2Slither.Core;

public partial class Board(int size)
{
    #region Board State
    public int Size { get; } = size;
    private readonly Cell[,] _grid = new Cell[size, size];
    public bool IsGameOver { get; private set; }
    public bool IsWin { get; private set; }
    
    #endregion
 
    #region Snake
    private readonly Snake _snake = new();
    public int SnakeLength => _snake.Length;
    public (int x, int y) SnakeHead => _snake.Head;
    public IReadOnlyList<(int x, int y)> SnakeBody => _snake.Body;
    public Direction CurrentDirection => _snake.Direction;
    #endregion
 
    #region Apples
    private readonly List<(int x, int y)> _greenApples = new();
    public IReadOnlyList<(int x, int y)> GreenApples => _greenApples;
    public (int x, int y) RedApple { get; private set; }
    #endregion

    #region Score & Progression
    private float _score = 0f;
    private float _lastReward = 0f;
    public int Steps { get; private set; }
    #endregion
 
    #region Read-only access
    public Cell GetCell(int x, int y) => _grid[x, y];
 
    public bool IsInside(int x, int y) => x >= 0 && x < Size && y >= 0 && y < Size;
    #endregion

    #region Geter & Seter
    public float GetScore() => _score;
    public float GetLastReward() => _lastReward;
    public bool GetGameOver() => IsGameOver;
    public bool GetWin() => IsWin;
    public void SetScore(float score) => _score += score;
    public void SetLastReward(float reward) => _lastReward = reward;
    public void IncrementStep() => Steps++;
    public void SetGameOver(bool GameOver) => IsGameOver = GameOver;
    public void SetWin(bool Win) => IsWin = Win;
    #endregion
}