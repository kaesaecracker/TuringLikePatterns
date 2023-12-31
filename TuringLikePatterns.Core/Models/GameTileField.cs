namespace TuringLikePatterns.Core.Models;

public sealed class GameTileField
{
    private readonly Dictionary<GamePosition, GameTile> _raw;
    private readonly GameBounds _bounds;

    public GameTileField(GameBounds bounds, Dictionary<GamePosition, GameTile>? raw = null)
    {
        _bounds = bounds;
        _raw = raw ?? new Dictionary<GamePosition, GameTile>();

        foreach (var pos in _raw.Keys)
            _bounds.ExpandTo(pos);
    }

    public int NonEmptyCount => _raw.Count;

    public GameTile? this[GamePosition position]
    {
        get => _raw.GetValueOrDefault(position);
        private set
        {
            ArgumentNullException.ThrowIfNull(value);
            _raw[position] = value;
            _bounds.ExpandTo(position);
        }
    }

    public GameTile GetOrCreate(GamePosition position)
    {
        if (_raw.TryGetValue(position, out var result))
            return result;

        return this[position] = new GameTile();
    }

    public IEnumerator<KeyValuePair<GamePosition, GameTile>> GetEnumerator() => _raw.GetEnumerator();
}
