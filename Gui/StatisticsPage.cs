using System.Globalization;

namespace TuringLikePatterns.Gui;

internal sealed class StatisticsPage : Grid
{
    public StatisticsPage(GameStateManager gameStateManager, TileDrawingArea drawArea)
    {
        _gameStateManager = gameStateManager;

        ColumnSpacing = 10;
        RowSpacing = 5;

        AttachGlobalStatistics();
        Attach(new Label(""), 0, _currentRow++, 2, 1);
        AttachCursorStatistics(drawArea);

        _gameStateManager.GameTickPassed += OnGameStateManagerTickPassed;
        drawArea.HoverTileChange += OnDrawAreaHoverTileChange;
    }

    private readonly GameStateManager _gameStateManager;
    private int _currentRow;
    private readonly Label _positionLabel = new("<Position>");

    private sealed record class Statistic(string Name, Func<GameState, string> TextFunc, Label Label);

    private sealed record class CursorStatistic(Quantity Quantity, Label Label);

    private readonly List<Statistic> _statistics =
    [
        new Statistic("Ticks", s => s.TickCount.ToString(CultureInfo.CurrentCulture), new Label()),
        new Statistic("Tiles live", s => s.Tiles.NonEmptyCount.ToString(CultureInfo.CurrentCulture), new Label()),
        new Statistic("Top left", s => s.Tiles.TopLeft.ToString(), new Label()),
        new Statistic("Bottom right", s => s.Tiles.BottomRight.ToString(), new Label()),
    ];

    private readonly List<CursorStatistic> _cursorStatistics = Quantity.All
        .Select(q => new CursorStatistic(q, new Label()))
        .ToList();

    private void OnDrawAreaHoverTileChange(object? _, HoverTileChangeEventArgs args)
    {
        _positionLabel.Text = args.Position.ToString();
        foreach (var (quantity, label) in _cursorStatistics)
        {
            var amount = (long)_gameStateManager.State.Tiles[args.Position][quantity];
            label.Text = amount.ToString(CultureInfo.CurrentCulture);
        }
    }

    private void OnGameStateManagerTickPassed(object? o, EventArgs eventArgs)
    {
        foreach (var (_, textFunc, label) in _statistics)
            label.Text = textFunc(_gameStateManager.State);
    }

    private void AttachGlobalStatistics()
    {
        foreach (var (name, textFunc, label) in _statistics)
        {
            Attach(new Label(name), 0, _currentRow, 1, 1);

            label.Text = textFunc(_gameStateManager.State);
            Attach(label, 1, _currentRow++, 1, 1);
        }
    }

    private void AttachCursorStatistics(TileDrawingArea drawArea)
    {
        Attach(new Label("=== Cursor ==="), 0, _currentRow++, 2, 1);

        Attach(new Label("Position"), 0, _currentRow, 1, 1);
        Attach(_positionLabel, 1, _currentRow++, 1, 1);

        foreach (var (quantity, label) in _cursorStatistics)
        {
            Attach(new Label(quantity.Name), 0, _currentRow, 1, 1);
            Attach(label, 1, _currentRow++, 1, 1);
        }
    }
}
