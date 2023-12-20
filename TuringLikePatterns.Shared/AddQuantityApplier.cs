using TuringLikePatterns.Models;
using TuringLikePatterns.TickPhases.Mutations;

namespace TuringLikePatterns.TickPhases.Appliers;

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
