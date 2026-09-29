using static Learn2Slither.Option;

class Program
{
    static void Main(string[] args)
    {
        /* Options */
        int sessions = 1;
        int size = 10;
        bool visualOn = true;
        string? savePath = null;
        string? loadPath = null;
        bool dontLearn = false;
        bool stepByStep = false;

        ParseArgs(args, ref sessions, ref size, ref visualOn, ref savePath, ref loadPath, ref dontLearn, ref stepByStep);
    }
}
