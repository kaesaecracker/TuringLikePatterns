using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;

namespace TuringLikePatterns.TickPhases;

public abstract class PoolingMutationProducer<TMut>(ObjectPool<TMut> pool) : IMutationProducer<TMut>
    where TMut : Mutation
{
    protected internal ObjectPool<TMut> Pool { get; } = pool;

    public abstract IEnumerable<TMut> ProduceMutations();

    public void ReturnToPool(IEnumerable<TMut> mutations)
    {
        foreach (var mutation in mutations)
            Pool.Return(mutation);
    }
}
