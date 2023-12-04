using System.Reactive.Subjects;

namespace TuringLikePatterns.GameState;

internal sealed class GameBounds
{
    public BehaviorSubject<GamePosition> TopLeft { get; } = new(new GamePosition(-1, -1));

    public BehaviorSubject<GamePosition> BottomRight { get; } = new(new GamePosition(1, 1));

    public void ExpandTo(GamePosition pos)
    {
        var tl = TopLeft.Value;
        if (pos.X < tl.X)
            tl = tl with { X = pos.X };
        if (pos.Y < tl.Y)
            tl = tl with { Y = pos.Y };
        if (tl != TopLeft.Value)
            TopLeft.OnNext(tl);

        var br = BottomRight.Value;
        if (pos.X > br.X)
            br = br with { X = pos.X };
        if (pos.Y > br.Y)
            br = br with { Y = pos.Y };
        if (br != BottomRight.Value)
            BottomRight.OnNext(br);
    }
}
