using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using SkiaSharp;
using TuringLikePatterns.Core;
using TuringLikePatterns.Core.Models;

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

    public static IServiceCollection AddChemistry(this IServiceCollection serviceProvider)
    {
        return serviceProvider
            .AddQuantity(new Quantity("water", SKColors.Aqua))
            .AddQuantity(new Quantity("hydrogen", SKColors.Blue))
            .AddQuantity(new Quantity("oxygen", SKColors.White))

            .AddSingleton<MakeWaterProducer>()
            .AddSingleton<BrownianMotionProducer>()
            .AddSingleton<AddQuantityApplier>()

            .AddTickPhase<BrownianMotionProducer, AddQuantityMutation, AddQuantityApplier>(1f, 0.01f)
            .AddTickPhase<MakeWaterProducer, AddQuantityMutation, AddQuantityApplier>(1f, 1f / 3f)

            .AddInfiniteObjectPool<AddQuantityMutation>();
    }

    public static IServiceCollection AddQuantity(this IServiceCollection serviceCollection, Quantity instance)
    {
        return serviceCollection
            .AddSingleton(instance)
            .AddKeyedSingleton(instance.Name, instance);
    }
}
