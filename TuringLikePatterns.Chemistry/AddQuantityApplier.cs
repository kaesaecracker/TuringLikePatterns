using TuringLikePatterns.API;

namespace TuringLikePatterns.Chemistry;

public sealed class AddQuantityApplier(IGameTileField tileField) : IMutationApplier<AddQuantityMutation>
{
    public void ApplyMutation(AddQuantityMutation mutation)
    {
        var tileToChange = tileField.GetOrCreate(mutation.Position);
        var oldValue = tileToChange[mutation.QuantityToChange];
        var newValue = oldValue + mutation.Amount;
        tileToChange[mutation.QuantityToChange] = newValue;
    }
}
