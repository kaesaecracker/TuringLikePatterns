using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TuringLikePatterns.Models;

internal sealed class GameBounds : IObservable<(GamePosition TopLeft, GamePosition BottomRight)>
{
    public BehaviorSubject<GamePosition> TopLeft { get; } = new(new GamePosition(-1, -1));

    public BehaviorSubject<GamePosition> BottomRight { get; } = new(new GamePosition(1, 1));

    private readonly IObservable<(GamePosition, GamePosition)> _combinedObservable;

    public GameBounds()
    {
        _combinedObservable = Observable
            .CombineLatest(TopLeft, BottomRight)
            .Select(gamePositions => (gamePositions[0], gamePositions[1]));
    }

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

    public IDisposable Subscribe(IObserver<(GamePosition, GamePosition)> observer) =>
        _combinedObservable.Subscribe(observer);
}
