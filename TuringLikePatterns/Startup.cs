using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuringLikePatterns.Chemistry;
using TuringLikePatterns.GameOfLife;
using TuringLikePatterns.Core;
using TuringLikePatterns.Views;
using TuringLikePatterns.ViewStates;

namespace TuringLikePatterns;

public static class Startup
{
    internal static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddTuringLikePatterns(builder =>
            {
                builder.AddCore()
                    .AddGameOfLife()
                    .AddChemistry();
            })
            .AddInfiniteObjectPool<AliveMutation>()
            .AddGui()
            .AddLogging(builder => builder
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole())

            .BuildServiceProvider();
    }

    private static IServiceCollection AddGui(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<ManualAddQuantityProducer>()
            .AddTickPhase<ManualAddQuantityProducer, AddQuantityMutation, AddQuantityApplier>()
            .AddSingleton<ZoomState>()
            .AddSingleton<TileAreaMouseState>()
            .AddSingleton<MainWindow>()
            .AddSingleton<GtkApplication>()
            .AddSingleton<TileDrawingArea>()
            .AddSingleton<IToolsPage, ActionsPage>()
            .AddSingleton<IToolsPage, StatisticsPage>()
            .AddSingleton<IToolsPage, ViewPage>();
    }
}
