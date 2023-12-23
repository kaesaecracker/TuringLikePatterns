using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.API;
using TuringLikePatterns.Core.Models;
using TuringLikePatterns.Core.TickPhases;

namespace TuringLikePatterns.Core;

public static class Extensions
{
    public static IServiceCollection AddStatistic(this IServiceCollection serviceCollection, string name,
        Func<IServiceProvider, Func<string>> updateFunProvider)
    {
        return serviceCollection.AddSingleton<Statistic>(sp => new Statistic(name, updateFunProvider(sp)));
    }

    public static IServiceCollection AddTickPhase<TProducer, TMutation, TConsumer>(
        this IServiceCollection serviceCollection)
        where TProducer : IMutationProducer<TMutation>
        where TMutation : class, IMutation
        where TConsumer : IMutationApplier<TMutation> =>
        serviceCollection.AddSingleton<ITickPhase, TickPhase<TProducer, TMutation, TConsumer>>();

    public static IServiceCollection AddTickPhase<TProducer, TMutation, TApplier>(
        this IServiceCollection serviceCollection, params object[] parameters)
        where TProducer : IMutationProducer<TMutation>
        where TMutation : class, IMutation
        where TApplier : IMutationApplier<TMutation>
    {
        return serviceCollection.AddSingleton<ITickPhase>(sp =>
        {
            var producer = ActivatorUtilities.CreateInstance<TProducer>(sp, parameters);
            var applier = ActivatorUtilities.CreateInstance<TApplier>(sp);
            return ActivatorUtilities.CreateInstance<TickPhase<TProducer, TMutation, TApplier>>(sp, producer, applier);
        });
    }

    public static IServiceCollection AddInfiniteObjectPool<T>(this IServiceCollection serviceCollection)
        where T : class, new()
    {
        return serviceCollection.AddSingleton<ObjectPool<T>>(sp =>
            new DefaultObjectPool<T>(new DefaultPooledObjectPolicy<T>(), int.MaxValue));
    }

    public static IServiceCollection AddCoreStatistics(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddStatistic("Top left", sp =>
            {
                var bounds = sp.GetRequiredService<GameBounds>();
                return () => bounds.TopLeft.ToString();
            })
            .AddStatistic("Bottom right", sp =>
            {
                var bounds = sp.GetRequiredService<GameBounds>();
                return () => bounds.BottomRight.ToString();
            })
            .AddStatistic("Ticks", sp =>
            {
                var ticker = sp.GetRequiredService<GameTicker>();
                return () => ticker.TickCount.ToString();
            })
            .AddStatistic("Tiles live", sp =>
            {
                var field = sp.GetRequiredService<GameTileField>();
                return () => field.NonEmptyCount.ToString(CultureInfo.CurrentCulture);
            });
    }

    public static IServiceCollection AddTicking(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<TickIncrementProducer>()
            .AddSingleton<TickIncrementApplier>()
            .AddTickPhase<TickIncrementProducer, TickIncrementMutation, TickIncrementApplier>();
    }


    public static IServiceCollection AddTuringLikePatterns(this IServiceCollection serviceCollection,
        Action<TuringLikePatternsBuilder> configure)
    {
        serviceCollection.AddOptions<Quantities>();
        var builder = new TuringLikePatternsBuilder(serviceCollection);
        configure(builder);
        return serviceCollection;
    }
}
