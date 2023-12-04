using System.Diagnostics;
using TuringLikePatterns.GameState;

namespace TuringLikePatterns.Mutations;

internal sealed class AddQuantityMutation : PooledGameStateMutation<AddQuantityMutation>
{
    private float _amount;
    private GamePosition _position;
    private Quantity? _quantityToChange;

    public static AddQuantityMutation Get(GamePosition position, Quantity quantityToChange, float amount)
    {
        var result = GetFromPool();
        result._position = position;
        result._quantityToChange = quantityToChange;
        result._amount = amount;
        return result;
    }

    protected override void InnerApply(GameTileField tileField, GameTicker ticker)
    {
        Trace.Assert(_quantityToChange != null);
        var tileToChange = tileField.GetOrCreate(_position);
        var oldValue = tileToChange[_quantityToChange];
        var newValue = oldValue + _amount;
        tileToChange[_quantityToChange] = newValue;
    }
}
