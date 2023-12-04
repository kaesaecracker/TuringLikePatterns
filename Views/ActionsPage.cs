using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TuringLikePatterns.GameState;
using TuringLikePatterns.Mutations;
using TuringLikePatterns.ViewStates;

namespace TuringLikePatterns.Views;

internal sealed class ActionsPage : IToolsPage, IDisposable
{
    private readonly Grid _grid;
    private readonly GameStateManager _gameStateManager;
    private readonly ManualMutationQueue _manualMutationQueue;
    private readonly ImmutableArray<Quantity> _quantities;
    private readonly ILogger<ActionsPage> _logger;

    private int _ticksPerClick = 1;
    private Quantity _selectedQuantity;
    private float _selectedAmount;

    string IToolsPage.Name => "Actions";
    Widget IToolsPage.Widget => _grid;

    public ActionsPage(
        GameStateManager gameStateManager,
        TileAreaMouseState drawingAreaMouseState,
        ManualMutationQueue manualMutationQueue,
        IEnumerable<Quantity> quantities,
        ILogger<ActionsPage> logger)
    {
        _logger = logger;
        _gameStateManager = gameStateManager;
        _manualMutationQueue = manualMutationQueue;
        _quantities = quantities.ToImmutableArray();
        _grid = new Grid();

        var currentRow = 0;
        var tpcSpinner = new SpinButton(1, 1000, 1);
        _grid.Attach(tpcSpinner, 0, currentRow, 1, 1);

        var tickButton = new Button(new Label("Tick"));
        _grid.Attach(tickButton, 1, currentRow++, 1, 1);

        _grid.Attach(new Label("=== Right click ==="), 0, currentRow++, 2, 1);

        _grid.Attach(new Label("Amount"), 0, currentRow, 1, 1);
        var amountSpinner = new SpinButton(1, 10000000000, 1);
        amountSpinner.Value = _selectedAmount;
        _grid.Attach(amountSpinner, 1, currentRow++, 1, 1);

        _grid.Attach(new Label("Type"), 0, currentRow, 1, 1);
        var quantitySelect = new ComboBox(_quantities.Select(q => q.Name).ToArray());
        _selectedQuantity = _quantities.First();
        quantitySelect.Active = 0;
        _grid.Attach(quantitySelect, 1, currentRow++, 1, 1);

        quantitySelect.Changed += QuantitySelectOnChanged;
        drawingAreaMouseState.RightClickTile.Subscribe(DrawingAreaOnTileRightClick);
        tickButton.Clicked += OnTickButtonClicked;
        amountSpinner.Changed += AmountSpinnerOnChanged;
        tpcSpinner.Changed += TpcSpinnerOnChanged;
    }

    private void TpcSpinnerOnChanged(object? sender, EventArgs e)
    {
        var spinner = sender as SpinButton;
        Trace.Assert(spinner != null);

        _ticksPerClick = spinner.ValueAsInt;
    }

    private void AmountSpinnerOnChanged(object? sender, EventArgs e)
    {
        var spinner = sender as SpinButton;
        Trace.Assert(spinner != null);

        _selectedAmount = (float)spinner.Value;
        _logger.LogDebug("changed to {SelectedAmount}", _selectedAmount);
    }

    private void QuantitySelectOnChanged(object? sender, EventArgs e)
    {
        var comboBox = sender as ComboBox;
        Trace.Assert(comboBox != null);
        _selectedQuantity = _quantities[comboBox.Active];
    }

    private void OnTickButtonClicked(object? sender, EventArgs eventArgs)
    {
        for (var i = 0; i < _ticksPerClick; i++)
            _gameStateManager.Tick();
    }

    private void DrawingAreaOnTileRightClick(GamePosition position)
    {
        _manualMutationQueue.Enqueue(AddQuantityMutation.Get(position, _selectedQuantity, _selectedAmount));
    }

    public void Dispose()
    {
        _grid.Dispose();
    }
}
