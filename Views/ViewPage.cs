using System.Reactive.Disposables;
using TuringLikePatterns.ViewStates;

namespace TuringLikePatterns.Views;

internal sealed class ViewPage : IToolsPage, IDisposable
{
    string IToolsPage.Name => "View";

    private readonly Grid _grid;
    Widget IToolsPage.Widget => _grid;

    public ViewPage(ZoomState zoomState)
    {
        var currentRow = 0;
        _grid = new Grid();

        var scale = new Scale(Orientation.Horizontal, 0.5d, 2.0d, 0.001d);
        _grid.Attach(scale, 0, currentRow++, 2, 1);

        var resetBtn = new Button(new Label("reset"));
        _grid.Attach(resetBtn, 0, currentRow++, 2, 1);

        var manualScaleSubscription = zoomState.ManualScale.Subscribe(d =>
        {
            scale.Value = d;
            resetBtn.Sensitive = d != ZoomState.DefaultScale;
        });
        scale.ValueChanged += (sender, args) => zoomState.ManualScale.OnNext(scale.Value);
        resetBtn.Clicked += (sender, args) => scale.Value = 1d;

        _compositeDisposable = new CompositeDisposable(_grid, scale, resetBtn, manualScaleSubscription);
    }

    private readonly CompositeDisposable _compositeDisposable;
    public void Dispose() => _compositeDisposable.Dispose();
}
