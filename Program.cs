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

    // Data
    services.AddSingleton(new Quantity("water", SKColors.Aqua));
    services.AddSingleton(new Quantity("hydrogen", SKColors.Blue));
    services.AddSingleton(new Quantity("oxygen", SKColors.White));
    services.AddSingleton<NamedDataResolver<Quantity>>();

    // Logic
    services.AddSingleton<IMutationGenerator, TickIncrementerMutationGenerator>();
    services.AddSingleton<IMutationGenerator>(sp =>
        new BrownianMotionMutationGenerator(1f, 0.01f));
    services.AddSingleton<IMutationGenerator>(sp =>
        ActivatorUtilities.CreateInstance<MakeWaterMutationGenerator>(sp, /*threshold*/1f, /*portionToReact*/0.333f));

    // State
    services.AddSingleton(GetDefaultInitialState);

    // GUI
    services.AddSingleton<MainWindow>();
    services.AddSingleton<GtkApplication>();
    services.AddSingleton<TileDrawingArea>();
    services.AddSingleton<StatisticsPage>();
    services.AddSingleton<ActionsPage>();

    return services.BuildServiceProvider();
}

static GameStateManager GetDefaultInitialState(IServiceProvider serviceProvider)
{
    var quantityResolver = serviceProvider.GetRequiredService<NamedDataResolver<Quantity>>();
    var initialState = new GameState(0, new GameStateTiles(new Dictionary<GamePosition, GameTile>
    {
        {
            new GamePosition(5, 5),
            new GameTile { [quantityResolver["oxygen"]] = 1000f }
        },
        {
            new GamePosition(9, 8),
            new GameTile { [quantityResolver["hydrogen"]] = 10000f }
        },
    }));
    return new GameStateManager(initialState, serviceProvider.GetServices<IMutationGenerator>());
}
