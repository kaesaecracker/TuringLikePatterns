namespace TuringLikePatterns.Core.Models;

public sealed class GameGrid<T>(GameBounds gameBounds)
    where T : struct
{
    private readonly Dictionary<GamePosition, T> _raw = new();

    public T this[GamePosition position]
    {
        get => _raw.GetValueOrDefault(position);
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (EqualityComparer<T>.Default.Equals(value, default))
            {
                _raw.Remove(position);
            }
            else
            {
                _raw[position] = value;
                gameBounds.ExpandTo(position);
            }
        }
    }
}
