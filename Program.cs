using GLib;
using Microsoft.Extensions.DependencyInjection;
using TuringLikePatterns;
using TuringLikePatterns.Gui;
using TuringLikePatterns.Mutations;

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
    var services = new ServiceCollection();
    services.AddSingleton(GetDefaultInitialState);
    services.AddSingleton<MainWindow>();
    services.AddSingleton<GtkApplication>();
    services.AddSingleton<TileDrawingArea>();
    services.AddSingleton<StatisticsPage>();
    services.AddSingleton<ActionsPage>();
    services.AddSingleton<IMutationGenerator, TickIncrementerMutationGenerator>();
    services.AddSingleton<IMutationGenerator>(sp =>
        new BrownianMotionMutationGenerator(1f, 0.01f));
    services.AddSingleton<IMutationGenerator>(sp => new MakeWaterMutationGenerator(1f, 0.333f));
    return services.BuildServiceProvider();
}

static GameStateManager GetDefaultInitialState(IServiceProvider serviceProvider)
{
    var initialState = new GameState(0, new GameStateTiles(new Dictionary<GamePosition, GameTile>
    {
        {
            new GamePosition(5, 5),
            new GameTile { [Quantity.Oxygen] = 1000f }
        },
        {
            new GamePosition(9, 8),
            new GameTile { [Quantity.Hydrogen] = 10000f }
        },
    }));
    return new GameStateManager(initialState, serviceProvider.GetServices<IMutationGenerator>());
}
