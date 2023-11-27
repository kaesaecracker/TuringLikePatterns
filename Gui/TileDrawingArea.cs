using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;

namespace TuringLikePatterns.Gui;

internal sealed class TileDrawingArea : SKDrawingArea
{
    private GameState? _lastKnownState;

    public TileDrawingArea(GameStateManager stateManager)
    {
        stateManager.GameTickPassed += StateManagerGameTickPassed;
    }

    private void StateManagerGameTickPassed(object? sender, GameTickPassedEventArgs e)
    {
        _lastKnownState = e.NewGameState;
        QueueDraw();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        var canvas = e.Surface.Canvas;
        var info = e.Info;

        using var autoRestore = new SKAutoCanvasRestore(canvas);
        canvas.Clear(SKColors.Black);
        if (_lastKnownState == null)
            return;

        var state = _lastKnownState;
        ApplyScale(state.Tiles.TopLeft, state.Tiles.BottomRight, info.Size, canvas);
        DrawTiles(state, canvas);
        DrawDebugSquare(canvas);
    }

    private static void ApplyScale(GamePosition topLeft, GamePosition bottomRight, SKSizeI canvasSize, SKCanvas canvas)
    {
        var logicalWidth = bottomRight.X - topLeft.X + 10;
        var logicalHeight = bottomRight.Y - topLeft.Y + 10;
        var sx = (float)canvasSize.Width / logicalWidth;
        var sy = (float)canvasSize.Height / logicalHeight;
        var s = Math.Min(sx, sy); // keep ratio
        canvas.Scale(s, s, topLeft.X, topLeft.Y);
    }

    private static void DrawTiles(GameState state, SKCanvas canvas)
    {
        using var tileMarkerPaint = new SKPaint();
        tileMarkerPaint.Color = SKColors.Gray;
        tileMarkerPaint.Style = SKPaintStyle.Stroke;

        foreach (var (pos, _) in state.Tiles)
            canvas.DrawRect(pos.X, pos.Y, 0.9f, 0.9f, tileMarkerPaint);
    }

    private static void DrawDebugSquare(SKCanvas canvas)
    {
        using var redPaint = new SKPaint();
        redPaint.Color = SKColors.Red;
        redPaint.Style = SKPaintStyle.Stroke;

        canvas.DrawRect(0f, 0f, 100f, 100f, redPaint);
    }
}
