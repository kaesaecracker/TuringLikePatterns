using TuringLikePatterns.Core.Models;
using TuringLikePatterns.Core.TickPhases;

namespace TuringLikePatterns.Chemistry;

public sealed class AddQuantityApplier(GameTileField tileField) : IMutationApplier<AddQuantityMutation>
{
    public void ApplyMutation(AddQuantityMutation mutation)
    {
        var tileToChange = tileField.GetOrCreate(mutation.Position);
        var oldValue = tileToChange[mutation.QuantityToChange];
        var newValue = oldValue + mutation.Amount;
        tileToChange[mutation.QuantityToChange] = newValue;
    }
}
