using System.Globalization;
using System.Runtime.CompilerServices;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using Window = Gtk.Window;

namespace TuringLikePatterns;

internal sealed class MainWindow : Window
{
    private readonly GameStateManager _gameStateManager;
    private readonly Grid _grid;
    private readonly SKDrawingArea _drawingArea;

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

        var child = new HBox();

        _drawingArea = new SKDrawingArea();
        _drawingArea.PaintSurface += OnPaintSurface;
        _gameStateManager.OnGameTickPassed += (sender, args) => _drawingArea.QueueDraw();
        _drawingArea.Show();

        child.PackStart(_drawingArea, true, true, 0);

        _grid = new Grid()
        {
            ColumnSpacing = 5,
            RowSpacing = 5,
            WidthRequest = 150,
        };

        var currentRow = 0;

        var statsLabel = new Label("== Statistics ==");
        statsLabel.Show();
        _grid.Attach(statsLabel, 0, currentRow++, 2, 1);

        AttachStatisticRow("Ticks", currentRow++, state => state.TickCount.ToString(CultureInfo.CurrentCulture));
        AttachStatisticRow("Tiles live", currentRow++, state => state.Tiles.NonEmptyCount.ToString(CultureInfo.CurrentCulture));
        AttachStatisticRow("Top left", currentRow++, state => state.Tiles.TopLeft.ToString());
        AttachStatisticRow("Bottom right", currentRow++, state => state.Tiles.BottomRight.ToString());

        var actionsLabel = new Label("== Actions ==");
        actionsLabel.Show();
        _grid.Attach(actionsLabel, 0, currentRow++, 2, 1);

        var tickButtonLabel = new Label("Tick");
        tickButtonLabel.Show();
        var tickButton = new Button(tickButtonLabel);
        tickButton.Clicked += (sender, args) => _gameStateManager.Tick();
        tickButton.Show();
        _grid.Attach(tickButton, 0, currentRow++, 2, 1);

        _grid.Show();
        child.PackEnd(_grid, false, false, 0);

        child.Show();
        Child = child;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        canvas.Save();
        canvas.Clear(SKColors.Black);

        var state = _gameStateManager.GetCurrentGameStateDoNotUseThis();
        var topLeft = state.Tiles.TopLeft;
        var bottomRight = state.Tiles.BottomRight;
        var logicalWidth = bottomRight.X - topLeft.X + 10;
        var logicalHeight = bottomRight.Y - topLeft.Y + 10;
        var sx = (float)info.Size.Width / logicalWidth;
        var sy = (float)info.Size.Height / logicalHeight;
        var s = Math.Min(sx, sy); // keep ratio
        canvas.Scale(s, s, topLeft.X,topLeft.Y);

        using var tileMarkerPaint = new SKPaint
        {
            Color = SKColors.Gray,
            Style = SKPaintStyle.Stroke
        };
        foreach (var (pos, tile) in state.Tiles)
        {
            canvas.DrawRect(pos.X, pos.Y, 0.9f, 0.9f, tileMarkerPaint);
        }

        using var redPaint = new SKPaint
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Stroke
        };
        canvas.DrawRect(0f,0f,100f,100f, redPaint);

        canvas.Restore();
    }

    private void AttachStatisticRow(string description, int top, Func<GameState, string> onChange)
    {
        var descriptionLabel = new Label(description);
        descriptionLabel.Show();
        _grid.Attach(descriptionLabel, 0, top, 1, 1);

        var valueLabel = new Label("<Value>");
        _gameStateManager.OnGameTickPassed += (sender, args) => { valueLabel.Text = onChange(args.NewGameState); };
        valueLabel.Show();
        _grid.Attach(valueLabel, 1, top, 1, 1);
    }
}
