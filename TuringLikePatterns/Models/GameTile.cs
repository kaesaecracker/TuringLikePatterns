namespace TuringLikePatterns.Models;

internal sealed class GameTile
{
    public Dictionary<Quantity, float> Raw { get; } = new();

    public float this[Quantity q]
    {
        get => Raw.GetValueOrDefault(q, 0f);
        set => Raw[q] = value;
    }

    internal Quantity? GetHighestQuantity() => Raw.Count != 0 ? Raw.MaxBy(pair => pair.Value).Key : null;

    public string ToDebugString() =>
        $"{{{string.Join(",", Raw.Select(kv => kv.Key + "=" + kv.Value).ToArray())}}}";
}
