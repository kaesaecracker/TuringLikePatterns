using Gdk;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;

namespace TuringLikePatterns.Gui;

internal sealed class TileDrawingArea : SKDrawingArea
{
    private GamePosition _lastMousePosition;
    private GamePosition _lastTopLeft;
    private GamePosition _lastBottomRight;

    private readonly GameStateManager _stateManager;

    private readonly Dictionary<Quantity, SKPaint> _quantityPaints;

    public TileDrawingArea(
        GameStateManager stateManager,
        IEnumerable<Quantity> allQuantities)
    {
        _stateManager = stateManager;
        _quantityPaints = allQuantities.ToDictionary(
            quantity => quantity,
            quantity =>
            {
                var paint = new SKPaint();
                paint.Color = quantity.Color;
                paint.Style = SKPaintStyle.Fill;
                return paint;
            });

        stateManager.GameTickPassed += StateManagerGameTickPassed;
        ButtonPressEvent += OnButtonPress;
        MotionNotifyEvent += OnMotion;
        AddEvents((int)EventMask.ButtonPressMask);
        AddEvents((int)EventMask.PointerMotionMask);
    }

    internal event EventHandler<HoverTileChangeEventArgs>? HoverTileChange;

    internal event EventHandler<TileClickEventArgs>? TileLeftClick;

    internal event EventHandler<TileClickEventArgs>? TileRightClick;

    private void OnMotion(object o, MotionNotifyEventArgs args)
    {
        var gamePosition = CanvasPointToGamePosition(args.Event.X, args.Event.Y);
        if (gamePosition == _lastMousePosition)
            return;
        _lastMousePosition = gamePosition;
        HoverTileChange?.Invoke(this, new HoverTileChangeEventArgs(gamePosition));
    }

    private void StateManagerGameTickPassed(object? sender, EventArgs e)
    {
        _lastTopLeft = _stateManager.State.Tiles.TopLeft;
        _lastBottomRight = _stateManager.State.Tiles.BottomRight;
        QueueDraw();
    }

    private void OnButtonPress(object o, ButtonPressEventArgs args)
    {
        var gamePosition = CanvasPointToGamePosition(args.Event.X, args.Event.Y);
        switch (args.Event.Button)
        {
            case 1:
                TileLeftClick?.Invoke(this, new TileClickEventArgs(gamePosition));
                break;
            case 3:
                TileRightClick?.Invoke(this, new TileClickEventArgs(gamePosition));
                break;
            default:
                Console.WriteLine($"unhandled mouse button {args.Event.Button}");
                break;
        }
    }

    private GamePosition CanvasPointToGamePosition(double x, double y)
    {
        var (translate, scale) = GetScalingInfo(CanvasSize.ToSizeI());
        return new GamePosition(
            (long)(x / scale - translate.X),
            (long)(y / scale - translate.Y)
        );
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        var canvas = e.Surface.Canvas;

        using var autoRestore = new SKAutoCanvasRestore(canvas);
        canvas.Clear(SKColors.Black);

        ApplyScale(e.Info.Size, canvas);
        DrawTiles(_stateManager.State, canvas, _quantityPaints);
    }

    private void ApplyScale(SKSizeI canvasSize, SKCanvas canvas)
    {
        var (translate, scale) = GetScalingInfo(canvasSize);
        canvas.Scale(scale);
        canvas.Translate(translate);
    }

    private (SKPoint Translate, float Scale) GetScalingInfo(SKSizeI canvasSize)
    {
        var logicalWidth = _lastBottomRight.X - _lastTopLeft.X + 1;
        var logicalHeight = _lastBottomRight.Y - _lastTopLeft.Y + 1;

        var sx = (float)canvasSize.Width / logicalWidth;
        var sy = (float)canvasSize.Height / logicalHeight;
        var s = Math.Min(sx, sy);
        return (new SKPoint(-_lastTopLeft.X, -_lastTopLeft.Y), s);
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
            if (highestQuantity != null && tile[highestQuantity] > 0)
            {
                var paint = quantityPaints[highestQuantity];
                canvas.DrawRect(position.X, position.Y, 1f, 1f, paint);
            }

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

internal sealed record class TileClickEventArgs(GamePosition Position);

internal sealed record class HoverTileChangeEventArgs(GamePosition Position);
