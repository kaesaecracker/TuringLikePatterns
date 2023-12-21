using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Core;
using TuringLikePatterns.Core.Models;

namespace TuringLikePatterns.Chemistry;

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
