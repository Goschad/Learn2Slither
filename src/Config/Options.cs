// ============================================================
// Option.cs
// Config — Command-line arguments: parsing, validation and default values.
// ============================================================

namespace Learn2Slither;

public static class Option
{
    const int MIN_SIZE = 5;
    const int MAX_SIZE = 30;

    static void Error(string message)
    {
        Console.Error.WriteLine($"Error: {message}");
        Environment.Exit(1);
    }

    static string GetValue(string[] args, ref int i)
    {
        if (i + 1 >= args.Length)
            Error($"{args[i]} requires a value");
        i++;
        return args[i];
    }

    static int ParsePositiveInt(string value, string option)
    {
        if (!int.TryParse(value, out int nbr) || nbr <= 0)
            Error($"{option} must be a positive integer (got '{value}')");
        return nbr;
    }

    static string ParsePath(string value, string option)
    {
        if (value.StartsWith("-"))
            Error($"{option} requires a file path (got '{value}')");
        return value;
    }

    public static void ParseArgs(string[] args, ref int sessions, ref int size, ref bool visualOn, ref string savePath, ref string loadPath, ref bool dontLearn, ref bool stepByStep, ref bool human, ref bool sans)
    {
        HashSet<string> seen = new();

        for (int i = 0; i < args.Length; i++)
        {
            string option = args[i];

            if (!seen.Add(option))
                Error($"{option} specified more than once");

            switch (option)
            {
                case "-sessions":
                    sessions = ParsePositiveInt(GetValue(args, ref i), option);
                    break;

                case "-size":
                    size = ParsePositiveInt(GetValue(args, ref i), option);
                    if (size < MIN_SIZE || size > MAX_SIZE)
                        Error($"{option} must be between {MIN_SIZE} and {MAX_SIZE} (got {size})");
                    break;

                case "-visual":
                    string visual = GetValue(args, ref i);
                    if (visual != "on" && visual != "off")
                        Error($"{option} must be 'on' or 'off' (got '{visual}')");
                    visualOn = visual == "on";
                    break;

                case "-save":
                    savePath = ParsePath(GetValue(args, ref i), option);
                    break;

                case "-load":
                    loadPath = ParsePath(GetValue(args, ref i), option);
                    break;

                case "-dontlearn":
                    dontLearn = true;
                    break;

                case "-step-by-step":
                    stepByStep = true;
                    break;

                case "-human":
                    human = true;
                    break;

                case "-sans":
                    sans = true;
                    break;

                default:
                    Error($"unknown option '{option}'");
                    break;
            }
        }
    }
}