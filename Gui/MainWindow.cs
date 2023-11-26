using System.Globalization;
using Window = Gtk.Window;

namespace TuringLikePatterns.Gui;

internal sealed class MainWindow : Window
{
    private readonly GameStateManager _gameStateManager;
    private readonly Grid _grid;

    public MainWindow(GameStateManager gameStateManager)
        : this(gameStateManager, new Builder("MainWindow.glade"))
    {
    }

    private MainWindow(GameStateManager gameStateManager, Builder builder)
        : base(builder.GetObject("MainWindow").Handle)
    {
        _gameStateManager = gameStateManager;

        builder.Autoconnect(this);
        DeleteEvent += (o, args) => Application.Quit();
        Name = "Turing-like Patterns";

        var drawingArea = new TileDrawingArea(_gameStateManager);
        drawingArea.Show();

        _grid = new Grid { ColumnSpacing = 5, RowSpacing = 5, WidthRequest = 150 };
        var currentRow = 0;
        currentRow = AttachStatistics(currentRow);
        currentRow = AttachActions(currentRow);
        _grid.Show();

        var child = new HBox();
        child.PackStart(drawingArea, true, true, 0);
        child.PackEnd(_grid, false, false, 0);
        child.Show();
        Child = child;
    }

    private int AttachActions(int currentRow)
    {
        var actionsLabel = new Label("== Actions ==");
        actionsLabel.Show();
        _grid.Attach(actionsLabel, 0, currentRow++, 2, 1);

        var tickButtonLabel = new Label("Tick");
        tickButtonLabel.Show();
        var tickButton = new Button(tickButtonLabel);
        tickButton.Clicked += (_, _) => _gameStateManager.Tick();
        tickButton.Show();
        _grid.Attach(tickButton, 0, currentRow++, 2, 1);
        return currentRow;
    }

    private int AttachStatistics(int currentRow)
    {
        var statsLabel = new Label("== Statistics ==");
        statsLabel.Show();
        _grid.Attach(statsLabel, 0, currentRow++, 2, 1);

        AttachStatisticRow("Ticks", currentRow++, state => state.TickCount.ToString(CultureInfo.CurrentCulture));
        AttachStatisticRow("Tiles live", currentRow++,
            state => state.Tiles.NonEmptyCount.ToString(CultureInfo.CurrentCulture));
        AttachStatisticRow("Top left", currentRow++, state => state.Tiles.TopLeft.ToString());
        AttachStatisticRow("Bottom right", currentRow++, state => state.Tiles.BottomRight.ToString());
        return currentRow;
    }

    private void AttachStatisticRow(string description, int top, Func<GameState, string> onChange)
    {
        var descriptionLabel = new Label(description);
        descriptionLabel.Show();
        _grid.Attach(descriptionLabel, 0, top, 1, 1);

        var valueLabel = new Label("<Value>");
        _gameStateManager.GameTickPassed += (sender, args) => { valueLabel.Text = onChange(args.NewGameState); };
        valueLabel.Show();
        _grid.Attach(valueLabel, 1, top, 1, 1);
    }
}
