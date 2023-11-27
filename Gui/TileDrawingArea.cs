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
        var clickedPoint = new SKPoint((float)args.Event.X, (float)args.Event.Y);
        var (translate, scale) = GetScalingInfo(CanvasSize.ToSizeI());
        clickedPoint = new SKPoint(clickedPoint.X / scale, clickedPoint.Y / scale);
        clickedPoint = new SKPoint(clickedPoint.X - translate.X, clickedPoint.Y - translate.Y);
        var gamePosition = new GamePosition((long)Math.Round(clickedPoint.X), (long)Math.Round(clickedPoint.Y));

        Trace.Assert(_lastKnownState != null);
        Console.WriteLine($"Clicked {gamePosition} with Tile {_lastKnownState.Tiles[gamePosition]}");
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
        var (translate, scale) = GetScalingInfo(canvasSize);
        canvas.Scale(scale);
        canvas.Translate(translate);
    }

    private (SKPoint Translate, float Scale) GetScalingInfo(SKSizeI canvasSize)
    {
        Trace.Assert(_lastKnownState != null);
        var topLeft = _lastKnownState.Tiles.TopLeft;
        var bottomRight = _lastKnownState.Tiles.BottomRight;

        var logicalWidth = bottomRight.X - topLeft.X + 1;
        var logicalHeight = bottomRight.Y - topLeft.Y + 1;

        var sx = (float)canvasSize.Width / logicalWidth;
        var sy = (float)canvasSize.Height / logicalHeight;
        var s = Math.Min(sx, sy);
        return (new SKPoint(-topLeft.X, -topLeft.Y), s);
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
            var highestQuantity = tile.GetHighestQuantity();
            canvas.DrawRect(position.X, position.Y, 1f, 1f, quantityPaints[highestQuantity]);
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
