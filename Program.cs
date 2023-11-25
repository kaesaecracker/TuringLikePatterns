using TuringLikePatterns.Mutations;

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

        var app = new Application("com.example.turing_like_patterns", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);
        app.AddWindow(win);

        Application.Run();
    }

    private static GameStateManager GetDefaultInitialState()
    {
        var tiles = new GameStateTiles(new Dictionary<GamePosition, GameTile>()
        {
            { new GamePosition(10, 10), new GameTile(1000, 1000) },
            { new GamePosition(25, 85), new GameTile(100, 100) },
        });
        var initialState = new GameState(TickCount: 0, Tiles: tiles);

        var mutationGenerators = new List<IMutationGenerator>
        {
            new TickIncrementerMutationGenerator(),
            new BrownianMotionMutationGenerator(),
        };

        return new GameStateManager(initialState, mutationGenerators);
    }
}
