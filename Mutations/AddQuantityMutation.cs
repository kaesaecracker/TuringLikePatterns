namespace TuringLikePatterns.Mutations;

internal sealed class AddQuantityMutation : PooledGameStateMutation<AddQuantityMutation>
{
    private float _amount;
    private GamePosition _position;
    private Quantity _quantityToChange;

    public static AddQuantityMutation Get(GamePosition position, Quantity quantityToChange, float amount)
    {
        var result = GetFromPool();
        result._position = position;
        result._quantityToChange = quantityToChange;
        result._amount = amount;
        return result;
    }

    public override GameState Apply(GameState gameState)
    {
        gameState = base.Apply(gameState);

        var newTile = gameState.Tiles[_position].WithChangedQuantity(_quantityToChange, _amount);
        var newTiles = gameState.Tiles.WithChangedTile(_position, newTile);
        return gameState with { Tiles = newTiles };
    }
}
