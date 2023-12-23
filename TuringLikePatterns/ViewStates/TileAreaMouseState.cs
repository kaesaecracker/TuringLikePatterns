using System.Reactive.Subjects;
using TuringLikePatterns.API;
using TuringLikePatterns.Core.Models;

namespace TuringLikePatterns.ViewStates;

internal sealed class TileAreaMouseState : ViewState
{
    public BehaviorSubject<GamePosition> HoveredTile { get; } = new(new GamePosition());

    public Subject<GamePosition> LeftClickTile { get; } = new();

    public Subject<GamePosition> RightClickTile { get; } = new();

    public TileAreaMouseState()
    {
        Disposables.Add(HoveredTile);
        Disposables.Add(LeftClickTile);
        Disposables.Add(RightClickTile);
    }
}
