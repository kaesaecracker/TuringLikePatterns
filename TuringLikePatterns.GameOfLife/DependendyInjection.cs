using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using TuringLikePatterns.TickPhases.Appliers;
using TuringLikePatterns.TickPhases.Mutations;

namespace TuringLikePatterns.GameOfLife;

public static class DependencyInjection
{
    public static IServiceCollection AddGameOfLife(this IServiceCollection services) => services
        .AddSingleton<GameOfLifeProducer>()
        .AddQuantity(new Quantity("Conway's life", SKColors.PaleGreen))
        .AddTickPhase<GameOfLifeProducer, AddQuantityMutation, AddQuantityApplier>();
}
