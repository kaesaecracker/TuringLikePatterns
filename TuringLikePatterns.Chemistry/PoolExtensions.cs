using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Shared.Models;
using TuringLikePatterns.Shared.TickPhases.Mutations;

namespace TuringLikePatterns.Shared.TickPhases;

public static class PoolExtensions
{
    public static AddQuantityMutation GetAddQuantity(this ObjectPool<AddQuantityMutation> pool,
        GamePosition position, Quantity quantityToChange, float amount)
    {
        var instance = pool.Get();
        instance.SetValues(position, quantityToChange, amount);
        return instance;
    }
}
