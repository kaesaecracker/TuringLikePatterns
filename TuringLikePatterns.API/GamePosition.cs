namespace TuringLikePatterns.API;

public readonly record struct GamePosition(long X, long Y)
{
    public override string ToString() => $"({X} | {Y})";
}
