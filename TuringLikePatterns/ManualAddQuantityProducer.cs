using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Chemistry;
using TuringLikePatterns.Core;
using TuringLikePatterns.Core.Models;
using TuringLikePatterns.Core.TickPhases;

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
