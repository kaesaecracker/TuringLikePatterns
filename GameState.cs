namespace TuringLikePatterns;

internal sealed record class GameState(
    long TickCount,
    GameStateTiles Tiles
)
{
    public long TickCount { get; set; } = TickCount;
}

internal sealed class GameStateTiles
{
    private readonly Dictionary<GamePosition, GameTile> _raw;

    public GameStateTiles(Dictionary<GamePosition, GameTile> raw)
    {
        ArgumentNullException.ThrowIfNull(raw);
        _raw = raw;

        TopLeft = new GamePosition(long.MaxValue, long.MaxValue);
        BottomRight = new GamePosition(long.MinValue, long.MinValue);

        foreach (var pos in _raw.Keys)
            RefreshBounds(pos);
    }

    private void RefreshBounds(GamePosition pos)
    {
        if (pos.X < TopLeft.X)
            TopLeft = TopLeft with { X = pos.X };
        if (pos.X > BottomRight.X)
            BottomRight = BottomRight with { X = pos.X };
        if (pos.Y < TopLeft.Y)
            TopLeft = TopLeft with { Y = pos.Y };
        if (pos.Y > BottomRight.Y)
            BottomRight = BottomRight with { Y = pos.Y };
    }

    public GamePosition TopLeft { get; private set; }

    public GamePosition BottomRight { get; private set; }

    public int NonEmptyCount => _raw.Count;

    public GameTile this[GamePosition pos]
    {
        get
        {
            if (_raw.TryGetValue(pos, out var result))
                return result;

            var newTile = new GameTile();
            this[pos] = newTile;
            return newTile;
        }

        private set
        {
            _raw[pos] = value;
            RefreshBounds(pos);
        }
    }

    public IEnumerator<KeyValuePair<GamePosition, GameTile>> GetEnumerator() => _raw.GetEnumerator();
}

internal readonly record struct GamePosition(long X, long Y)
{
    internal GamePosition Top() => this with { Y = Y - 1 };

    internal GamePosition Bottom() => this with { Y = Y + 1 };

    internal GamePosition Left() => this with { X = X - 1 };

    internal GamePosition Right() => this with { X = X + 1 };

    public override string ToString() => $"({X} | {Y})";
}

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
