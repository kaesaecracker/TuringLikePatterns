using System.Collections.Generic;
using System.Linq;

namespace TuringLikePatterns.Models;

public sealed class GameTile
{
    public Dictionary<Quantity, float> Raw { get; } = new();

    public float this[Quantity q]
    {
        get => Raw.GetValueOrDefault(q, 0f);
        set => Raw[q] = value;
    }

    public Quantity? GetHighestQuantity() => Raw.Count != 0 ? Raw.MaxBy(pair => pair.Value).Key : null;

    public string ToDebugString() =>
        $"{{{string.Join(",", Raw.Select(kv => kv.Key + "=" + kv.Value).ToArray())}}}";
}
