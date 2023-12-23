using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.API;

namespace TuringLikePatterns.Chemistry;

public static class Extensions
{
    public static AddQuantityMutation GetAddQuantity(this ObjectPool<AddQuantityMutation> pool,
        GamePosition position, Quantity quantityToChange, float amount)
    {
        var instance = pool.Get();
        instance.SetValues(position, quantityToChange, amount);
        return instance;
    }

    public static ITuringLikePatternsBuilder AddChemistry(this ITuringLikePatternsBuilder builder)
    {
        builder
            .AddPlane(new Quantity("water", Color.Aqua))
            .AddPlane(new Quantity("hydrogen", Color.Blue))
            .AddPlane(new Quantity("oxygen", Color.White));
        builder.Services
            .AddSingleton<MakeWaterProducer>()
            .AddSingleton<BrownianMotionProducer>()
            .AddSingleton<AddQuantityApplier>()
            .AddTickPhase<BrownianMotionProducer, AddQuantityMutation, AddQuantityApplier>(1f, 0.01f)
            .AddTickPhase<MakeWaterProducer, AddQuantityMutation, AddQuantityApplier>(1f, 1f / 3f)
            .AddInfiniteObjectPool<AddQuantityMutation>();
        return builder;
    }
}
