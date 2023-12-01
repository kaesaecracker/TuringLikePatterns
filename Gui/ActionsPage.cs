namespace TuringLikePatterns.Gui;

internal sealed class ActionsPage : Grid
{
    public ActionsPage(GameStateManager gameStateManager)
    private int _currentRow;

    public ActionsPage(GameStateManager gameStateManager, TileDrawingArea drawingArea)
    {
        var currentRow = 0;
        AttachManualTicker(gameStateManager);
    }

    private void AttachManualTicker(GameStateManager gameStateManager)
    {
        var tpcSpinner = new SpinButton(1, 1000, 1);
        Attach(tpcSpinner, 0, _currentRow, 1, 1);

        var tickButton = new Button(new Label("Tick"));
        Attach(tickButton, 1, _currentRow++, 1, 1);

        tickButton.Clicked += (_, _) =>
        {
            for (var i = 0; i < tpcSpinner.ValueAsInt; i++)
                gameStateManager.Tick();
        };
    }
}
