using System.Collections.Immutable;
using System.Diagnostics;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using TuringLikePatterns.API;
using TuringLikePatterns.Core;
using TuringLikePatterns.ViewStates;

namespace TuringLikePatterns.Views;

internal sealed class ActionsPage : IToolsPage, IDisposable
{
    private readonly Grid _grid = new();
    private readonly GameStateManager _gameStateManager;
    private readonly ManualAddQuantityProducer _manualAddQuantityProducer;
    private readonly ImmutableArray<Quantity> _quantities;
    private readonly ILogger<ActionsPage> _logger;
    private readonly CompositeDisposable _disposables;

    private int _ticksPerClick = 1;
    private Quantity _selectedQuantity;
    private float _selectedAmount;

    string IToolsPage.Name => "Actions";
    Widget IToolsPage.Widget => _grid;

    public ActionsPage(
        GameStateManager gameStateManager,
        TileAreaMouseState drawingAreaMouseState,
        ManualAddQuantityProducer manualAddQuantityProducer,
        IEnumerable<Quantity> quantities,
        ILogger<ActionsPage> logger)
    {
        _logger = logger;
        _gameStateManager = gameStateManager;
        _manualAddQuantityProducer = manualAddQuantityProducer;
        _quantities = quantities.ToImmutableArray();

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
        tickButton.Clicked += OnTickButtonClicked;
        amountSpinner.Changed += AmountSpinnerOnChanged;
        tpcSpinner.Changed += TpcSpinnerOnChanged;

        drawingAreaMouseState.RightClickTile.Subscribe(DrawingAreaOnTileRightClick);

        _disposables = new CompositeDisposable(_grid, tpcSpinner, tickButton, quantitySelect, amountSpinner);
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

        // TODO: this does not fire when value is changed by typing instead of +/- buttons
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
        _manualAddQuantityProducer.Enqueue(position, _selectedQuantity, _selectedAmount);
    }

    public void Dispose() => _disposables.Dispose();
}
