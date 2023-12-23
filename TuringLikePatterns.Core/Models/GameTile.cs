using System.Collections;
using System.Linq;
using TuringLikePatterns.API;

namespace TuringLikePatterns.Core.Models;

public sealed class GameTile: IGameTile
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

    public IEnumerator<(Quantity, float)> GetEnumerator()
    {
        return Raw.Select(kvp => (kvp.Key, kvp.Value)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
