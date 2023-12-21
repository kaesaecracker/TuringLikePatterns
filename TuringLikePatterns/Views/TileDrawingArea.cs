using Gdk;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using TuringLikePatterns.Core;
using TuringLikePatterns.Core.Models;
using TuringLikePatterns.ViewStates;

namespace TuringLikePatterns.Views;

internal sealed class TileDrawingArea : SKDrawingArea
{
    private GamePosition _lastTopLeft;
    private GamePosition _lastBottomRight;

    private readonly GameStateManager _stateManager;

    private readonly Dictionary<Quantity, SKPaint> _quantityPaints;
    private readonly ILogger _logger;
    private readonly ZoomState _zoomState;
    private readonly GameBounds _gameBounds;
    private readonly GameTileField _field;

    public TileDrawingArea(
        GameStateManager stateManager,
        IEnumerable<Quantity> allQuantities,
        ILogger<TileDrawingArea> logger,
        ZoomState zoomState,
        TileAreaMouseState mouseState,
        GameBounds gameBounds,
        GameTileField field)
    {
        _logger = logger;
        _stateManager = stateManager;
        _zoomState = zoomState;
        _gameBounds = gameBounds;
        _field = field;

        _quantityPaints = allQuantities.ToDictionary(
            quantity => quantity,
            quantity =>
            {
                var paint = new SKPaint();
                paint.Color = quantity.Color;
                paint.Style = SKPaintStyle.Fill;
                return paint;
            });

        _lastTopLeft = gameBounds.TopLeft.Value;
        _lastBottomRight = gameBounds.BottomRight.Value;

        _stateManager.GameTickPassed += StateManagerGameTickPassed;

        ButtonPressEvent += (o, args) =>
        {
            var gamePosition = CanvasPointToGamePosition(args.Event.X, args.Event.Y);
            switch (args.Event.Button)
            {
                case 1:
                    mouseState.LeftClickTile.OnNext(gamePosition);
                    break;
                case 3:
                    mouseState.RightClickTile.OnNext(gamePosition);
                    break;
                default:
                    _logger.LogDebug("unhandled mouse button {Button}", args.Event.Button);
                    break;
            }
        };
        MotionNotifyEvent += (o, args) =>
        {
            var gamePosition = CanvasPointToGamePosition(args.Event.X, args.Event.Y);
            mouseState.HoveredTile.OnNext(gamePosition);
        };
        //ConfigureEvent += OnConfigureEvent;

        AddEvents((int)EventMask.ButtonPressMask);
        AddEvents((int)EventMask.PointerMotionMask);
    }

    private void StateManagerGameTickPassed(object? sender, EventArgs e)
    {
        _lastTopLeft = _gameBounds.TopLeft.Value;
        _lastBottomRight = _gameBounds.BottomRight.Value;
        QueueDraw();
    }

    private GamePosition CanvasPointToGamePosition(double x, double y)
    {
        var (translate, scale) = GetScalingInfo(CanvasSize.ToSizeI());
        return new GamePosition(
            (long)Math.Round(x / scale - translate.X, MidpointRounding.ToNegativeInfinity),
            (long)Math.Round(y / scale - translate.Y, MidpointRounding.ToNegativeInfinity)
        );
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        var canvas = e.Surface.Canvas;

        using var autoRestore = new SKAutoCanvasRestore(canvas);
        canvas.Clear(SKColors.Black);

        ApplyScale(e.Info.Size, canvas);
        DrawTiles(canvas);
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
        var s = Math.Min(sx, sy) * _zoomState.ManualScale.Value;
        return (new SKPoint(-_lastTopLeft.X, -_lastTopLeft.Y), (float)s);
    }

    private void DrawTiles(SKCanvas canvas)
    {
        using var tileMarkerPaint = new SKPaint();
        tileMarkerPaint.Color = SKColors.Gray;
        tileMarkerPaint.Style = SKPaintStyle.Stroke;

        foreach (var (position, tile) in _field)
        {
            var highestQuantity = tile.GetHighestQuantity();
            if (highestQuantity != null && tile[highestQuantity] > 0)
            {
                var paint = _quantityPaints[highestQuantity];
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
