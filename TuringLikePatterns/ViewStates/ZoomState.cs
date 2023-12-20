using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace TuringLikePatterns.ViewStates;

internal sealed class ZoomState : ViewState
{
    public const double DefaultScale = 1.0d;

    public BehaviorSubject<double> ManualScale { get; } = new(DefaultScale);
    public BehaviorSubject<double> AutoScale { get; } = new(DefaultScale);


    public ZoomState()
    {
        Disposables.Add(ManualScale);
        Disposables.Add(AutoScale);
    }
}
