using System.Reactive.Disposables;

namespace TuringLikePatterns.ViewStates;

public class ViewState : IDisposable
{
    protected CompositeDisposable Disposables { get; } = new();

    public void Dispose()
    {
        Disposables.Dispose();
        GC.SuppressFinalize(this);
    }
}
