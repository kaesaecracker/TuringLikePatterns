using System.Diagnostics;
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

        ApplyScale(e.Info.Size, canvas);
        DrawTiles(_lastKnownState, canvas, _quantityPaints);
    }

    private void ApplyScale(SKSizeI canvasSize, SKCanvas canvas)
    {
        Trace.Assert(_lastKnownState != null);
        var topLeft = _lastKnownState.Tiles.TopLeft;
        var bottomRight = _lastKnownState.Tiles.BottomRight;

        var logicalWidth = bottomRight.X - topLeft.X + 1;
        var logicalHeight = bottomRight.Y - topLeft.Y + 1;

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
