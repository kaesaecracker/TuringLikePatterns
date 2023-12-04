namespace TuringLikePatterns;

internal sealed record class Statistic(string Name, Func<string> TextFunc)
{
    // TODO: move Label out of here
    public Label Label { get; } = new(Name);
}
