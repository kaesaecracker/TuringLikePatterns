using System.Globalization;
using Window = Gtk.Window;

namespace TuringLikePatterns.Gui;

internal sealed class MainWindow : Window
{
    private readonly TileDrawingArea _drawingArea;

    public MainWindow(TileDrawingArea drawingArea, StatisticsPage statisticsPage, ActionsPage actionsPage)
        : base("Turing-like Patterns")
    {
        _drawingArea = drawingArea;

        DeleteEvent += (_, _) => Application.Quit();

        var notebook = new Notebook();
        notebook.AppendPage(statisticsPage, new Label("Statistics"));
        notebook.AppendPage(actionsPage, new Label("Actions"));

        var child = new HBox();
        child.PackStart(_drawingArea, true, true, 0);
        child.PackEnd(notebook, false, false, 0);
        child.ShowAll();
        Child = child;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _drawingArea.Dispose();
    }
}
