using System.Collections;
using System.Linq;
using Microsoft.Extensions.ObjectPool;

namespace TuringLikePatterns;

internal readonly record struct GameState(
    long TickCount,
    GameStateTiles Tiles
);

internal sealed class GameStateTiles : IEnumerable<KeyValuePair<GamePosition, GameTile>>, IDisposable
{
    private readonly AutoPoolingDictionary<GamePosition, GameTile> _raw;

    public GameStateTiles(AutoPoolingDictionary<GamePosition, GameTile> raw)
    {
        ArgumentNullException.ThrowIfNull(raw);
        _raw = raw;

        var min = new GamePosition(long.MaxValue, long.MaxValue);
        var max = new GamePosition(long.MinValue, long.MinValue);

        foreach (var (pos, _) in _raw)
        {
            if (pos.X < min.X)
                min = min with { X = pos.X };
            if (pos.X > max.X)
                max = max with { X = pos.X };
            if (pos.Y < min.Y)
                min = min with { Y = pos.Y };
            if (pos.Y > max.Y)
                max = max with { Y = pos.Y };
        }

        TopLeft = min;
        BottomRight = max;
    }

    public GamePosition TopLeft { get; }
    public GamePosition BottomRight { get; }

    public int NonEmptyCount => _raw.Count;

    public GameTile this[GamePosition pos] => _raw.GetValueOrDefault(pos) ?? new GameTile();

    IEnumerator IEnumerable.GetEnumerator() => _raw.GetEnumerator();

    IEnumerator<KeyValuePair<GamePosition, GameTile>> IEnumerable<KeyValuePair<GamePosition, GameTile>>.
        GetEnumerator() => _raw.GetEnumerator();

    public GameStateTiles WithChangedTile(GamePosition position, GameTile newTile)
    {
        var newTiles = AutoPoolingDictionary<GamePosition, GameTile>.GetFromPool(_raw);
        newTiles[position] = newTile;
        return new GameStateTiles(newTiles);
    }

    public void Dispose()
    {
        _raw.Dispose();
    }
}

internal readonly record struct GamePosition(long X, long Y)
{
    internal GamePosition Top() => this with { Y = Y - 1 };

    internal GamePosition Bottom() => this with { Y = Y + 1 };

    internal GamePosition Left() => this with { X = X - 1 };

    internal GamePosition Right() => this with { X = X + 1 };

    public override string ToString() => $"({X} | {Y})";
}

internal sealed class GameTile(AutoPoolingDictionary<Quantity, float>? raw = null) : IDisposable
{
    public float this[Quantity q] => raw?.GetValueOrDefault(q, 0f) ?? 0f;

    public GameTile WithChangedQuantity(Quantity quantity, float amount)
    {
        var newDict = AutoPoolingDictionary<Quantity, float>.GetFromPool(raw);
        newDict[quantity] = this[quantity] + amount;
        return new GameTile(newDict);
    }

    public IEnumerable<KeyValuePair<Quantity, float>> Quantities =>
        raw ?? Enumerable.Empty<KeyValuePair<Quantity, float>>();

    public void Dispose() => raw?.Dispose();
}
