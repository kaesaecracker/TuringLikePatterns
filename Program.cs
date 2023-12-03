using GLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuringLikePatterns;
using TuringLikePatterns.Mutations;
using TuringLikePatterns.Views;
using TuringLikePatterns.ViewStates;

var serviceProvider = ConfigureServices();
Gtk.Application.Init();

var app = serviceProvider.GetRequiredService<GtkApplication>();
app.Register(Cancellable.Current);

var window = serviceProvider.GetRequiredService<MainWindow>();
app.AddWindow(window);
window.Show();

Gtk.Application.Run();
return;

static ServiceProvider ConfigureServices()
{
    return new ServiceCollection()
        // Data
        .AddQuantity(new Quantity("water", SKColors.Aqua))
        .AddQuantity(new Quantity("hydrogen", SKColors.Blue))
        .AddQuantity(new Quantity("oxygen", SKColors.White))
        .AddQuantity(new Quantity("Conway's life", SKColors.PaleGreen))

        // Logic
        .AddMutationGenerator<TickIncrementerMutationGenerator>()
        .AddMutationGenerator<BrownianMotionMutationGenerator>(1f, 0.01f)
        .AddMutationGenerator<MakeWaterMutationGenerator>( /*threshold*/1f, /*portionToReact*/0.333f)
        .AddMutationGenerator<ManualMutationQueue>()
        .AddMutationGenerator<ConwaysGameOfLifeMutationGenerator>()

        // State
        .AddSingleton<ZoomState>()
        .AddSingleton<GameStateManager>()

        // GUI
        .AddSingleton<MainWindow>()
        .AddSingleton<GtkApplication>()
        .AddSingleton<TileDrawingArea>()
        .AddSingleton<ActionsPage>()
        .AddSingleton<StatisticsPage>()
        .AddSingleton<IToolsPage, ActionsPage>()
        .AddSingleton<IToolsPage, StatisticsPage>()

        // Technical stuff
        .AddLogging(builder => builder
            .SetMinimumLevel(LogLevel.Trace)
            .AddConsole())

        // Done
        .BuildServiceProvider();
}
