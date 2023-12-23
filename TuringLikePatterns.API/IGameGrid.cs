namespace TuringLikePatterns.API;

public interface IGameGrid<T>
{
    T this[GamePosition position] { get; set; }
}
