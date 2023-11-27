using Gdk;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;

namespace TuringLikePatterns.Gui;

internal sealed class TileDrawingArea : SKDrawingArea
{
    private GameState? _lastKnownState;

    private readonly Dictionary<Quantity, SKPaint> _quantityPaints = Quantity.All.ToDictionary(
        quantity => quantity,
        quantity =>
        {
            var paint = new SKPaint();
            paint.Color = quantity.Color;
            paint.Style = SKPaintStyle.Fill;
            return paint;
        });

    public TileDrawingArea(GameStateManager stateManager)
    {
        stateManager.GameTickPassed += StateManagerGameTickPassed;
        ButtonPressEvent += OnButtonPressEvent;
        AddEvents((int)EventMask.ButtonPressMask);
    }

    private void StateManagerGameTickPassed(object? sender, GameTickPassedEventArgs e)
    {
        _lastKnownState = e.NewGameState;
        QueueDraw();
    }

    private void OnButtonPressEvent(object o, ButtonPressEventArgs args)
    {
        Console.WriteLine($"clicked on position ({args.Event.X}, {args.Event.Y})");
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        var canvas = e.Surface.Canvas;

        using var autoRestore = new SKAutoCanvasRestore(canvas);
        canvas.Clear(SKColors.Black);
        if (_lastKnownState == null)
            return;

        ApplyScale(_lastKnownState.Tiles.TopLeft, _lastKnownState.Tiles.BottomRight, e.Info.Size, canvas);
        DrawTiles(_lastKnownState, canvas, _quantityPaints);
    }

    private static void ApplyScale(GamePosition topLeft, GamePosition bottomRight, SKSizeI canvasSize, SKCanvas canvas)
    {
        var logicalWidth = bottomRight.X - topLeft.X + 10;
        var logicalHeight = bottomRight.Y - topLeft.Y + 10;
        var sx = (float)canvasSize.Width / logicalWidth;
        var sy = (float)canvasSize.Height / logicalHeight;
        var s = Math.Min(sx, sy); // keep ratio
        canvas.Scale(s);
        canvas.Translate(-topLeft.X, -topLeft.Y);
    }

    private static void DrawTiles(
        GameState state,
        SKCanvas canvas,
        IReadOnlyDictionary<Quantity, SKPaint> quantityPaints)
    {
        using var tileMarkerPaint = new SKPaint();
        tileMarkerPaint.Color = SKColors.Gray;
        tileMarkerPaint.Style = SKPaintStyle.Stroke;

        foreach (var (position, tile) in state.Tiles)
        {
            canvas.DrawRect(position.X, position.Y, 1f, 1f, quantityPaints[tile.GetHighestQuantity()]);
            canvas.DrawRect(position.X, position.Y, 1f, 1f, tileMarkerPaint);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;

        foreach (var paint in _quantityPaints.Values)
            paint.Dispose();
        _quantityPaints.Clear();
    }
}
