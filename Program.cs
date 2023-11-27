using GLib;
using TuringLikePatterns.Gui;
using TuringLikePatterns.Mutations;
using Application = Gtk.Application;

namespace TuringLikePatterns;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.Init();

        var gsm = GetDefaultInitialState();
        var win = new MainWindow(gsm);
        win.Show();

        var app = new Application("com.example.turing_like_patterns", ApplicationFlags.None);
        app.Register(Cancellable.Current);
        app.AddWindow(win);

        Application.Run();
    }

    private static GameStateManager GetDefaultInitialState()
    {
        var tiles = new GameStateTiles(new Dictionary<GamePosition, GameTile>
        {
            {
                new GamePosition(10, 10),
                new GameTile(new Dictionary<Quantity, float> { { Quantity.Oxygen, 1000 } })
            },
            {
                new GamePosition(25, 30),
                new GameTile(new Dictionary<Quantity, float> { { Quantity.Hydrogen, 1000 } })
            },
        });
        var initialState = new GameState(0, tiles);

        var mutationGenerators = new List<IMutationGenerator>
        {
            new TickIncrementerMutationGenerator(),
            new BrownianMotionMutationGenerator(1f, 0.01f),
        };

        return new GameStateManager(initialState, mutationGenerators);
    }
}
