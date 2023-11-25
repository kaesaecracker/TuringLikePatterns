using TuringLikePatterns.Mutations;

namespace TuringLikePatterns;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.Init();

        var app = new Application("com.example.turing_like_patterns", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);

        var initialState = new GameState(
            TickCount: 0,
            Tiles: new GameStateTiles(new Dictionary<GamePosition, GameTile>()
            {
                { new GamePosition(10, 10), new GameTile(1000, 1000, 0) },
                { new GamePosition(25, 85), new GameTile(100, 100, 0) },
            })
        );
        var mutationGenerators = new List<IMutationGenerator>
        {
            new TickIncrementerMutationGenerator(),
            new BrownianMotionMutationGenerator(),
        };
        var gsm = new GameStateManager(initialState, mutationGenerators);

        var win = new MainWindow(gsm);
        app.AddWindow(win);

        win.Show();
        Application.Run();
    }
}
