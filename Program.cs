using GLib;
using TuringLikePatterns.Gui;
using TuringLikePatterns.Mutations;
using Application = Gtk.Application;
using Microsoft.Extensions.DependencyInjection;

namespace TuringLikePatterns;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.Init();

        var services = new ServiceCollection();
        services.AddSingleton(GetDefaultInitialState());
        services.AddSingleton<MainWindow>();
        services.AddSingleton<GtkApplication>();
        services.AddSingleton<TileDrawingArea>();
        services.AddSingleton<StatisticsPage>();
        services.AddSingleton<ActionsPage>();

        var serviceProvider = services.BuildServiceProvider();

        var win = serviceProvider.GetRequiredService<MainWindow>();
        win.Show();
        var app = serviceProvider.GetRequiredService<GtkApplication>();
        app.Register(Cancellable.Current);
        app.AddWindow(win);

        Application.Run();
    }

    private static GameStateManager GetDefaultInitialState()
    {
        var tiles = new GameStateTiles(new Dictionary<GamePosition, GameTile>
        {
            {
                new GamePosition(5, 5),
                new GameTile { [Quantity.Oxygen] = 1000f }
            },
            {
                new GamePosition(9, 8),
                new GameTile { [Quantity.Hydrogen] = 10000f }
            },
        });
        var initialState = new GameState(0, tiles);

        var mutationGenerators = new List<IMutationGenerator>
        {
            new TickIncrementerMutationGenerator(),
            new BrownianMotionMutationGenerator(1f, 0.01f),
            new MakeWaterMutationGenerator(1f, 0.333f),
        };

        return new GameStateManager(initialState, mutationGenerators);
    }
}
