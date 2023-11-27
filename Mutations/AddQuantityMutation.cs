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

    protected override void InnerApply(GameState gameState)
    {
        gameState.Tiles[_position][_quantityToChange] += _amount;
    }
}
