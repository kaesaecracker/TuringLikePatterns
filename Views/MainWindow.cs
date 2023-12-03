using Window = Gtk.Window;

namespace TuringLikePatterns.Views;

internal sealed class MainWindow : Window
{
    private readonly TileDrawingArea _drawingArea;

    public MainWindow(TileDrawingArea drawingArea, IEnumerable<IToolsPage> toolPages)
        : base("Turing-like Patterns")
    {
        _drawingArea = drawingArea;
        DeleteEvent += (_, _) => Application.Quit();

        var notebook = new Notebook();
        foreach (var toolPage in toolPages)
            notebook.AppendPage(toolPage.Widget, new Label(toolPage.Name));

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
