using System.Collections;
using System.Linq;
using TuringLikePatterns.API;

namespace TuringLikePatterns.Core.Models;

public sealed class GameTileField : IGameTileField
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

    public IGameTile GetOrCreate(GamePosition position)
    {
        if (_raw.TryGetValue(position, out var result))
            return result;

        return this[position] = new GameTile();
    }

    public IEnumerator<(GamePosition, IGameTile)> GetEnumerator()
    {
        return _raw.Select(kvp => (kvp.Key, (IGameTile)kvp.Value)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
