using System.Collections;

namespace TuringLikePatterns.API;

[Obsolete]
public interface IGameTileField: IEnumerable<(GamePosition, IGameTile)>
{
    IGameTile GetOrCreate(GamePosition mutationPosition);
}

public interface IGameTile : IEnumerable<(Quantity, float)>
{
    float this[Quantity mutationQuantityToChange] { get; set; }

    Quantity? GetHighestQuantity();
}
