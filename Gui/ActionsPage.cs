using System.Collections.Immutable;
using System.Diagnostics;
using TuringLikePatterns.Mutations;

namespace TuringLikePatterns.Gui;

internal sealed class ActionsPage : Grid
{
    private readonly GameStateManager _gameStateManager;
    private readonly ManualMutationQueue _manualMutationQueue;
    private readonly ImmutableArray<Quantity> _quantities;

    private int _ticksPerClick = 1;
    private Quantity _selectedQuantity;
    private float _selectedAmount = 100;

    public ActionsPage(
        GameStateManager gameStateManager,
        TileDrawingArea drawingArea,
        ManualMutationQueue manualMutationQueue,
        IEnumerable<Quantity> quantities)
    {
        _gameStateManager = gameStateManager;
        _manualMutationQueue = manualMutationQueue;
        _quantities = quantities.ToImmutableArray();

        var currentRow = 0;
        var tpcSpinner = new SpinButton(1, 1000, 1);
        Attach(tpcSpinner, 0, currentRow, 1, 1);

        var tickButton = new Button(new Label("Tick"));
        Attach(tickButton, 1, currentRow++, 1, 1);

        Attach(new Label("=== Right click ==="), 0, currentRow++, 2, 1);

        Attach(new Label("Amount"), 0, currentRow, 1, 1);
        var amountSpinner = new SpinButton(100, 10000000000, 10);
        amountSpinner.Value = _selectedAmount;
        Attach(amountSpinner, 1, currentRow++, 1, 1);

        Attach(new Label("Type"), 0, currentRow, 1, 1);
        var quantitySelect = new ComboBox(_quantities.Select(q => q.Name).ToArray());
        _selectedQuantity = _quantities.First();
        quantitySelect.Active = 0;
        Attach(quantitySelect, 1, currentRow++, 1, 1);

        quantitySelect.Changed += QuantitySelectOnChanged;
        drawingArea.TileRightClick += DrawingAreaOnTileRightClick;
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

    private void DrawingAreaOnTileRightClick(object? sender, TileClickEventArgs e)
    {
        var position = e.Position;
        _manualMutationQueue.Enqueue(AddQuantityMutation.Get(position, _selectedQuantity, _selectedAmount));
    }
}
