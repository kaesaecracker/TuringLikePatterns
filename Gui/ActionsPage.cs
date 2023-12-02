using TuringLikePatterns.Mutations;

namespace TuringLikePatterns.Gui;

internal sealed class ActionsPage : Grid
{
    private readonly GameStateManager _gameStateManager;
    private readonly ManualMutationQueue _manualMutationQueue;
    private readonly IEnumerable<Quantity> _quantities;
    private readonly SpinButton _tpcSpinner;
    private readonly SpinButton _amountSpinner;
    private int _currentRow;

    public ActionsPage(
        GameStateManager gameStateManager,
        TileDrawingArea drawingArea,
        ManualMutationQueue manualMutationQueue,
        IEnumerable<Quantity> quantities)
    {
        _gameStateManager = gameStateManager;
        _manualMutationQueue = manualMutationQueue;
        _quantities = quantities;

        _tpcSpinner = new SpinButton(1, 1000, 1);
        _amountSpinner = new SpinButton(100, 10000000000, 10);

        drawingArea.TileRightClick += DrawingAreaOnTileRightClick;

        AttachManualTicker();
        AttachRightClickControl();
    }

    private void AttachManualTicker()
    {
        Attach(_tpcSpinner, 0, _currentRow, 1, 1);

        var tickButton = new Button(new Label("Tick"));
        Attach(tickButton, 1, _currentRow++, 1, 1);

        tickButton.Clicked += OnTickButtonClicked;
    }

    private void OnTickButtonClicked(object? sender, EventArgs eventArgs)
    {
        for (var i = 0; i < _tpcSpinner.ValueAsInt; i++)
            _gameStateManager.Tick();
    }

    private void AttachRightClickControl()
    {
        Attach(new Label("=== Right click ==="), 0, _currentRow++, 2, 1);

        Attach(new Label("Amount"), 0, _currentRow, 1, 1);
        Attach(_amountSpinner, 1, _currentRow++, 1, 1);
    }

    private void DrawingAreaOnTileRightClick(object? sender, TileClickEventArgs e)
    {
        var position = e.Position;
        var amount = (float)_amountSpinner.Value;
        var quantity = _quantities.First(); // TODO make selectable
        _manualMutationQueue.Enqueue(AddQuantityMutation.Get(position, quantity, amount));
    }
}
