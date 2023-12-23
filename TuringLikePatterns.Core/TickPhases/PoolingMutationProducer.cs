using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.API;

namespace TuringLikePatterns.Core.TickPhases;

public abstract class PoolingMutationProducer<TMut>(ObjectPool<TMut> pool) : IMutationProducer<TMut>
    where TMut : class, IMutation
{
    protected ObjectPool<TMut> Pool { get; } = pool;

    public abstract IEnumerable<TMut> ProduceMutations();

    public void ReturnToPool(IEnumerable<TMut> mutations)
    {
        foreach (var mutation in mutations)
            Pool.Return(mutation);
    }
}
