using System.Globalization;

namespace TuringLikePatterns.Gui;

internal sealed class StatisticsPage : Grid
{
    private int _currentRow;

    public StatisticsPage(GameStateManager gameStateManager, TileDrawingArea drawArea)
    {
        ColumnSpacing = 10;
        RowSpacing = 5;

        AttachGlobalStatistics(gameStateManager);
        Attach(new Label(""), 0, _currentRow++, 2, 1);
        AttachCursorStatistics(gameStateManager, drawArea);
    }

    private void AttachGlobalStatistics(GameStateManager gameStateManager)
    {
        Dictionary<string, (Func<GameState, string> TextFunc, Label Label)> statistics = new()
        {
            ["Ticks"] = (s => s.TickCount.ToString(CultureInfo.CurrentCulture), new Label()),
            ["Tiles live"] = (s => s.Tiles.NonEmptyCount.ToString(CultureInfo.CurrentCulture), new Label()),
            ["Top left"] = (s => s.Tiles.TopLeft.ToString(), new Label()),
            ["Bottom right"] = (s => s.Tiles.BottomRight.ToString(), new Label()),
        };

        foreach (var (name, (textFunc, label)) in statistics)
        {
            Attach(new Label(name), 0, _currentRow, 1, 1);

            label.Text = textFunc(gameStateManager.State);
            Attach(label, 1, _currentRow++, 1, 1);
        }

        gameStateManager.GameTickPassed += (_, _) =>
        {
            foreach (var (textFunc, label) in statistics.Values)
                label.Text = textFunc(gameStateManager.State);
        };
    }

    private void AttachCursorStatistics(GameStateManager gameStateManager, TileDrawingArea drawArea)
    {
        Attach(new Label("=== Cursor ==="), 0, _currentRow++, 2, 1);

        Attach(new Label("Position"), 0, _currentRow, 1, 1);
        var positionLabel = new Label("<Position>");
        Attach(positionLabel, 1, _currentRow++, 1, 1);

        var labelDict = new Dictionary<Quantity, Label>(Quantity.All.Count);
        foreach (var quantity in Quantity.All)
        {
            Attach(new Label(quantity.Name), 0, _currentRow, 1, 1);
            var label = new Label($"<{quantity.Name}>");
            labelDict[quantity] = label;
            Attach(label, 1, _currentRow++, 1, 1);
        }

        drawArea.HoverTileChange += (_, args) =>
        {
            positionLabel.Text = args.Position.ToString();
            foreach (var (quantity, label) in labelDict)
            {
                var amount = (long)gameStateManager.State.Tiles[args.Position][quantity];
                label.Text = amount.ToString(CultureInfo.CurrentCulture);
            }
        };
    }
}
