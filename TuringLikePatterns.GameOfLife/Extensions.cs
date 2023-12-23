using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.API;

namespace TuringLikePatterns.GameOfLife;

public static class Extensions
{
    public static ITuringLikePatternsBuilder AddGameOfLife(this ITuringLikePatternsBuilder builder)
    {
        builder.Services
            .AddKeyedSingleton<IGameGrid<bool>>(Constants.GameOfLife)
            .AddSingleton<GameOfLifeProducer>()
            .AddSingleton<AliveMutationApplier>()
            .AddTickPhase<GameOfLifeProducer, AliveMutation, AliveMutationApplier>();
        return builder;
    }

    internal static AliveMutation GetAliveMutation(this ObjectPool<AliveMutation> pool, GamePosition position,
        bool isAlive)
    {
        var instance = pool.Get();
        instance.Position = position;
        instance.Alive = isAlive;
        return instance;
    }
}
