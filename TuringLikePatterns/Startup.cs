using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuringLikePatterns.GameOfLife;
using TuringLikePatterns.Shared;
using TuringLikePatterns.Shared.Models;
using TuringLikePatterns.Shared.TickPhases.Appliers;
using TuringLikePatterns.Shared.TickPhases.Mutations;
using TuringLikePatterns.Shared.TickPhases.Producers;
using TuringLikePatterns.Views;
using TuringLikePatterns.ViewStates;

namespace TuringLikePatterns;

public static class Startup
{
    internal static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddGameOfLife()

            // Data
            .AddQuantity(new Quantity("water", SKColors.Aqua))
            .AddQuantity(new Quantity("hydrogen", SKColors.Blue))
            .AddQuantity(new Quantity("oxygen", SKColors.White))

            // TODO: make statistics react instead of being pulled?
            .AddStatistic("Ticks", sp =>
            {
                var ticker = sp.GetRequiredService<GameTicker>();
                return () => ticker.TickCount.ToString(CultureInfo.CurrentCulture);
            })
            .AddStatistic("Tiles live", sp =>
            {
                var field = sp.GetRequiredService<GameTileField>();
                return () => field.NonEmptyCount.ToString(CultureInfo.CurrentCulture);
            })
            .AddStatistic("Top left", sp =>
            {
                var bounds = sp.GetRequiredService<GameBounds>();
                return () => bounds.TopLeft.Value.ToString();
            })
            .AddStatistic("Bottom right", sp =>
            {
                var bounds = sp.GetRequiredService<GameBounds>();
                return () => bounds.BottomRight.Value.ToString();
            })

            // Producers
            .AddSingleton<ManualAddQuantityProducer>()
            .AddSingleton<MakeWaterProducer>()
            .AddSingleton<BrownianMotionProducer>()
            .AddSingleton<TickIncrementProducer>()

            // Appliers
            .AddSingleton<AddQuantityApplier>()
            .AddSingleton<TickIncrementApplier>()

            // Phases
            .AddTickPhase<BrownianMotionProducer, AddQuantityMutation, AddQuantityApplier>(1f, 0.01f)
            .AddTickPhase<MakeWaterProducer, AddQuantityMutation, AddQuantityApplier>(1f, 1f / 3f)
            .AddTickPhase<TickIncrementProducer, TickIncrementMutation, TickIncrementApplier>()
            .AddTickPhase<ManualAddQuantityProducer, AddQuantityMutation, AddQuantityApplier>()

            // State
            .AddSingleton<ZoomState>()
            .AddSingleton<TileAreaMouseState>()
            .AddSingleton<GameBounds>()
            .AddSingleton<GameTileField>()
            .AddSingleton<GameTicker>()
            .AddSingleton<GameStateManager>()

            // GUI
            .AddSingleton<MainWindow>()
            .AddSingleton<GtkApplication>()
            .AddSingleton<TileDrawingArea>()
            .AddSingleton<IToolsPage, ActionsPage>()
            .AddSingleton<IToolsPage, StatisticsPage>()
            .AddSingleton<IToolsPage, ViewPage>()

            // Technical stuff
            .AddLogging(builder => builder
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole())
            .AddInfiniteObjectPool<AddQuantityMutation>()

            // Done
            .BuildServiceProvider();
    }
}
