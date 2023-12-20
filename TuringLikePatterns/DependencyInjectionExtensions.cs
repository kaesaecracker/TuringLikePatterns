using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.TickPhases;

namespace TuringLikePatterns;

internal static class DependencyInjectionExtensions
{
    internal static IServiceCollection AddQuantity(this IServiceCollection serviceCollection, Quantity instance) =>
        serviceCollection
            .AddSingleton(instance)
            .AddKeyedSingleton(instance.Name, instance);

    internal static IServiceCollection AddStatistic(this IServiceCollection serviceCollection, string name,
        Func<IServiceProvider, Func<string>> updateFunProvider)
    {
        return serviceCollection.AddSingleton<Statistic>(sp => new Statistic(name, updateFunProvider(sp)));
    }

    internal static IServiceCollection AddTickPhase<TProducer, TMutation, TConsumer>(
        this IServiceCollection serviceCollection)
        where TProducer : IMutationProducer<TMutation>
        where TMutation : Mutation
        where TConsumer : IMutationApplier<TMutation> =>
        serviceCollection.AddSingleton<ITickPhase, TickPhase<TProducer, TMutation, TConsumer>>();

    internal static IServiceCollection AddTickPhase<TProducer, TMutation, TApplier>(
        this IServiceCollection serviceCollection, params object[] parameters)
        where TProducer : IMutationProducer<TMutation>
        where TMutation : Mutation
        where TApplier : IMutationApplier<TMutation>
    {
        return serviceCollection.AddSingleton<ITickPhase>(sp =>
        {
            var producer = ActivatorUtilities.CreateInstance<TProducer>(sp, parameters);
            var applier = ActivatorUtilities.CreateInstance<TApplier>(sp);
            return ActivatorUtilities.CreateInstance<TickPhase<TProducer, TMutation, TApplier>>(sp, producer, applier);
        });
    }

    internal static IServiceCollection AddInfiniteObjectPool<T>(this IServiceCollection serviceCollection)
        where T : class, new()
    {
        return serviceCollection.AddSingleton<ObjectPool<T>>(sp =>
            new DefaultObjectPool<T>(new DefaultPooledObjectPolicy<T>(), int.MaxValue));
    }
}
