using System.Globalization;

namespace TuringLikePatterns.Views;

internal sealed class StatisticsPage : IToolsPage, IDisposable
{
    private sealed record class Statistic(string Name, Func<GameState, string> TextFunc, Label Label);

    private sealed record class CursorStatistic(Quantity Quantity, Label Label);

    public StatisticsPage(GameStateManager gameStateManager, TileDrawingArea drawArea,
        IEnumerable<Quantity> allQuantities)
    {
        _gameStateManager = gameStateManager;
        _cursorStatistics = allQuantities
            .Select(q => new CursorStatistic(q, new Label()))
            .ToList();

        _grid.ColumnSpacing = 10;
        _grid.RowSpacing = 5;

        AttachGlobalStatistics();
        _grid.Attach(new Label(""), 0, _currentRow++, 2, 1);
        AttachCursorStatistics();

        _gameStateManager.GameTickPassed += OnGameStateManagerTickPassed;
        drawArea.HoverTileChange += OnDrawAreaHoverTileChange;
    }

    string IToolsPage.Name => "Statistics";
    Widget IToolsPage.Widget => _grid;

    private readonly Grid _grid = new();
    private readonly GameStateManager _gameStateManager;
    private int _currentRow;
    private readonly Label _positionLabel = new("<Position>");

    private readonly List<CursorStatistic> _cursorStatistics;
    private readonly List<Statistic> _statistics =
    [
        new Statistic("Ticks", s => s.TickCount.ToString(CultureInfo.CurrentCulture), new Label()),
        new Statistic("Tiles live", s => s.Tiles.NonEmptyCount.ToString(CultureInfo.CurrentCulture), new Label()),
        new Statistic("Top left", s => s.Tiles.TopLeft.ToString(), new Label()),
        new Statistic("Bottom right", s => s.Tiles.BottomRight.ToString(), new Label()),
    ];

    private void OnDrawAreaHoverTileChange(object? _, HoverTileChangeEventArgs args)
    {
        _positionLabel.Text = args.Position.ToString();
        var tile = _gameStateManager.State.Tiles[args.Position];

        foreach (var (quantity, label) in _cursorStatistics)
            label.Text = Math.Round(tile?[quantity] ?? 0f).ToString(CultureInfo.CurrentCulture);
    }

    private void OnGameStateManagerTickPassed(object? _, EventArgs eventArgs)
    {
        foreach (var (_, textFunc, label) in _statistics)
            label.Text = textFunc(_gameStateManager.State);
    }

    private void AttachGlobalStatistics()
    {
        foreach (var (name, textFunc, label) in _statistics)
        {
            _grid.Attach(new Label(name), 0, _currentRow, 1, 1);

            label.Text = textFunc(_gameStateManager.State);
            _grid.Attach(label, 1, _currentRow++, 1, 1);
        }
    }

    private void AttachCursorStatistics()
    {
        _grid.Attach(new Label("=== Cursor ==="), 0, _currentRow++, 2, 1);

        _grid.Attach(new Label("Position"), 0, _currentRow, 1, 1);
        _grid.Attach(_positionLabel, 1, _currentRow++, 1, 1);

        foreach (var (quantity, label) in _cursorStatistics)
        {
            _grid.Attach(new Label(quantity.Name), 0, _currentRow, 1, 1);
            _grid.Attach(label, 1, _currentRow++, 1, 1);
        }
    }

    public void Dispose()
    {
        _grid.Dispose();
        _positionLabel.Dispose();
    }
}
