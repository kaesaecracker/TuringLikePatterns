using System.Globalization;
using Window = Gtk.Window;

namespace TuringLikePatterns.Gui;

internal sealed class MainWindow : Window
{
    private readonly TileDrawingArea _drawingArea;
    private readonly GameStateManager _gameStateManager;
    private readonly Grid _grid;

    public MainWindow(GameStateManager gameStateManager, TileDrawingArea drawingArea)
        : this(gameStateManager, drawingArea, new Builder("MainWindow.glade"))
    {
    }

    private MainWindow(GameStateManager gameStateManager, TileDrawingArea drawingArea, Builder builder)
        : base(builder.GetObject("MainWindow").Handle)
    {
        _gameStateManager = gameStateManager;
        _drawingArea = drawingArea;

        builder.Autoconnect(this);
        DeleteEvent += (_, _) => Application.Quit();
        Name = "Turing-like Patterns";

        _grid = new Grid { ColumnSpacing = 5, RowSpacing = 5, WidthRequest = 150 };
        var currentRow = 0;
        AttachStatistics(ref currentRow);
        AttachActions(ref currentRow);
        AttachCursorInfo(ref currentRow);

        var child = new HBox { Spacing = 5 };
        child.PackStart(_drawingArea, true, true, 0);
        child.PackEnd(_grid, false, false, 0);
        child.ShowAll();
        Child = child;
    }

    private void AttachCursorInfo(ref int currentRow)
    {
        _grid.Attach(new Label("== Cursor =="), 0, currentRow++, 2, 1);

        _grid.Attach(new Label("Position"), 0, currentRow, 1, 1);
        var positionLabel = new Label("<Position>");
        _grid.Attach(positionLabel, 1, currentRow++, 1, 1);

        var labelDict = new Dictionary<Quantity, Label>(Quantity.All.Count);
        foreach (var quantity in Quantity.All)
        {
            _grid.Attach(new Label(quantity.Name), 0, currentRow, 1, 1);
            var label = new Label($"<{quantity.Name}>");
            labelDict[quantity] = label;
            _grid.Attach(label, 1, currentRow++, 1, 1);
        }

        _drawingArea.HoverTileChange += (_, args) =>
        {
            positionLabel.Text = args.Position.ToString();
            foreach (var (quantity, label) in labelDict)
            {
                var amount = (long)_gameStateManager.State.Tiles[args.Position][quantity];
                label.Text = amount.ToString(CultureInfo.CurrentCulture);
            }
        };
    }

    private void AttachActions(ref int currentRow)
    {
        _grid.Attach(new Label("== Actions =="), 0, currentRow++, 2, 1);

        var tpcSpinner = new SpinButton(1, 1000, 1);
        _grid.Attach(tpcSpinner, 0, currentRow, 1, 1);

        var tickButton = new Button(new Label("Tick"));
        _grid.Attach(tickButton, 1, currentRow++, 1, 1);

        tickButton.Clicked += (_, _) =>
        {
            for (var i = 0; i < tpcSpinner.ValueAsInt; i++)
                _gameStateManager.Tick();
        };
    }

    private void AttachStatistics(ref int currentRow)
    {
        _grid.Attach(new Label("== Statistics =="), 0, currentRow++, 2, 1);

        Dictionary<string, (Func<GameState, string> TextFunc, Label Label)> statistics = new()
        {
            ["Ticks"] = (s => s.TickCount.ToString(CultureInfo.CurrentCulture), new Label()),
            ["Tiles live"] = (s => s.Tiles.NonEmptyCount.ToString(CultureInfo.CurrentCulture), new Label()),
            ["Top left"] = (s => s.Tiles.TopLeft.ToString(), new Label()),
            ["Bottom right"] = (s => s.Tiles.BottomRight.ToString(), new Label()),
        };

        foreach (var (name, (textFunc, label)) in statistics)
        {
            _grid.Attach(new Label(name), 0, currentRow, 1, 1);

            label.Text = textFunc(_gameStateManager.State);
            _grid.Attach(label, 1, currentRow++, 1, 1);
        }

        _gameStateManager.GameTickPassed += (_, _) =>
        {
            foreach (var (textFunc, label) in statistics.Values)
                label.Text = textFunc(_gameStateManager.State);
        };
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _grid.Dispose();
        _drawingArea.Dispose();
    }
}
