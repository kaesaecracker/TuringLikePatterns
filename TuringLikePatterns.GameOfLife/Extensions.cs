using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Core;
using TuringLikePatterns.Core.Models;

namespace TuringLikePatterns.GameOfLife;

public static class Extensions
{
    public static IServiceCollection AddGameOfLife(this IServiceCollection services) => services
        .AddKeyedSingleton<GameGrid<bool>>(Constants.GameOfLife)
        .AddSingleton<GameOfLifeProducer>()
        .AddInfiniteObjectPool<AliveMutation>()
        .AddSingleton<AliveMutationApplier>()
        .AddTickPhase<GameOfLifeProducer, AliveMutation, AliveMutationApplier>();

    internal static AliveMutation GetAliveMutation(this ObjectPool<AliveMutation> pool, GamePosition position,
        bool isAlive)
    {
        var instance = pool.Get();
        instance.Position = position;
        instance.Alive = isAlive;
        return instance;
    }
}
