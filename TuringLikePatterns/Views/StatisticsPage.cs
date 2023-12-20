using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TuringLikePatterns.Models;
using TuringLikePatterns.ViewStates;

namespace TuringLikePatterns.Views;

internal sealed class StatisticsPage : IToolsPage, IDisposable
{
    private sealed record class CursorStatistic(Quantity Quantity, Label Label);

    public StatisticsPage(
        GameStateManager gameStateManager,
        TileAreaMouseState drawAreaMouseState,
        IEnumerable<Quantity> allQuantities,
        IEnumerable<Statistic> allStatistics,
        GameTileField field
    )
    {
        _gameStateManager = gameStateManager;
        _field = field;
        _statistics = allStatistics.ToDictionary(s => s, s => new Label(s.TextFunc()));

        _cursorStatistics = allQuantities
            .Select(q => new CursorStatistic(q, new Label()))
            .ToList();

        _grid.ColumnSpacing = 10;
        _grid.RowSpacing = 5;

        AttachGlobalStatistics();
        _grid.Attach(new Label(""), 0, _currentRow++, 2, 1);
        AttachCursorStatistics();

        _gameStateManager.GameTickPassed += OnGameStateManagerTickPassed;

        var subscription = drawAreaMouseState.HoveredTile
            .DistinctUntilChanged()
            .Subscribe(OnDrawAreaHoverTileChange);

        _disposables = new CompositeDisposable(subscription, _grid, _positionLabel);
    }

    string IToolsPage.Name => "Statistics";
    Widget IToolsPage.Widget => _grid;

    private readonly GameStateManager _gameStateManager;
    private readonly GameTileField _field;

    private readonly CompositeDisposable _disposables;

    private int _currentRow;
    private readonly Grid _grid = new();
    private readonly Label _positionLabel = new("<Position>");

    private readonly List<CursorStatistic> _cursorStatistics;
    private readonly Dictionary<Statistic, Label> _statistics;

    private void OnDrawAreaHoverTileChange(GamePosition position)
    {
        _positionLabel.Text = position.ToString();
        var tile = _field[position];

        foreach (var (quantity, label) in _cursorStatistics)
            label.Text = Math.Round(tile?[quantity] ?? 0f).ToString(CultureInfo.CurrentCulture);
    }

    private void OnGameStateManagerTickPassed(object? _, EventArgs eventArgs)
    {
        foreach (var (statistic, label) in _statistics)
            label.Text = statistic.TextFunc();
    }

    private void AttachGlobalStatistics()
    {
        foreach (var (statistic, label) in _statistics)
        {
            _grid.Attach(new Label(statistic.Name), 0, _currentRow, 1, 1);

            label.Text = statistic.TextFunc();
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
        _disposables.Dispose();
    }
}
