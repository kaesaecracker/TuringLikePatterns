using System.Collections;

namespace TuringLikePatterns;

internal readonly record struct GameState(
    long TickCount,
    GameStateTiles Tiles
);

internal sealed class GameStateTiles : IEnumerable<KeyValuePair<GamePosition, GameTile>>
{
    private readonly IReadOnlyDictionary<GamePosition, GameTile> _raw;

    public GameStateTiles(IReadOnlyDictionary<GamePosition, GameTile> raw)
    {
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

    public GameTile this[GamePosition pos] => _raw.GetValueOrDefault(pos);

    IEnumerator IEnumerable.GetEnumerator() => _raw.GetEnumerator();

    IEnumerator<KeyValuePair<GamePosition, GameTile>> IEnumerable<KeyValuePair<GamePosition, GameTile>>.GetEnumerator()
        => _raw.GetEnumerator();
}

internal readonly record struct GamePosition(long X, long Y)
{
    internal GamePosition Top() => this with { Y = Y - 1 };
    internal GamePosition Bottom() => this with { Y = Y + 1 };
    internal GamePosition Left() => this with { X = X - 1 };
    internal GamePosition Right() => this with { X = X + 1 };

    public override string ToString()
    {
        return $"({X} | {Y})";
    }
}

internal readonly record struct GameTile(
    float Hydrogen = 0,
    float Oxygen = 0,
    float Water = 0
);
