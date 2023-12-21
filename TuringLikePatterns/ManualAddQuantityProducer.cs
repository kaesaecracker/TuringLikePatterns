using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Shared;
using TuringLikePatterns.Shared.Models;
using TuringLikePatterns.Shared.TickPhases;
using TuringLikePatterns.Shared.TickPhases.Mutations;

namespace TuringLikePatterns;

public sealed class ManualAddQuantityProducer(ObjectPool<AddQuantityMutation> pool)
    : PoolingMutationProducer<AddQuantityMutation>(pool)
{
    private readonly Queue<AddQuantityMutation> _queuedMutations = new();

    public override IEnumerable<AddQuantityMutation> ProduceMutations()
    {
        while (_queuedMutations.TryDequeue(out var result))
            yield return result;
    }

    public void Enqueue(GamePosition position, Quantity selectedQuantity, float selectedAmount)
    {
        _queuedMutations.Enqueue(Pool.GetAddQuantity(position, selectedQuantity, selectedAmount));
    }
}
