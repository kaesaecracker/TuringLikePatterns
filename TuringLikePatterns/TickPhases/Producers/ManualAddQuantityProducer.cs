using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Models;
using TuringLikePatterns.TickPhases.Mutations;

namespace TuringLikePatterns.TickPhases.Producers;

internal sealed class ManualAddQuantityProducer(ObjectPool<AddQuantityMutation> pool)
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
