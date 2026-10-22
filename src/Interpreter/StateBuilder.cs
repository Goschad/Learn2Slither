using Learn2Slither.Core;

namespace Learn2Slither.Interpreter;

public partial class StateBuilder
{
    Board _board;

    public StateBuilder(Board board)
    {
        _board = board;
    }

    public string GetState()
    {
        string up = LookDirection(0, -1);
        string down = LookDirection(0, 1);
        string left = LookDirection(-1, 0);
        string right = LookDirection(1, 0);

        return up + "|" + down + "|" + left + "|" + right;
    }

    private char CellToChar(int x, int y)
    {
        return _board.GetCell(x, y) switch
        {
            Cell.Empty      => '0',
            Cell.SnakeHead  => 'H',
            Cell.SnakeBody  => 'S',
            Cell.GreenApple => 'G',
            Cell.RedApple   => 'R',
            _ => '0'
        };
    }

    private string LookDirection(int dx, int dy)
    {
        var (hx, hy) = _board.SnakeHead;
        string result = "";
        int x = hx + dx;
        int y = hy + dy;

        while (_board.IsInside(x, y))
        {
            result += CellToChar(x, y);
            x += dx;
            y += dy;
        }
        result += 'W';
        return result;
    }
}