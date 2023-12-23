using GLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuringLikePatterns;
using TuringLikePatterns.Views;
using TuringLikePatterns.ViewStates;

var serviceProvider = Startup.ConfigureServices();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

Gtk.Application.Init();

var app = serviceProvider.GetRequiredService<GtkApplication>();
app.Register(Cancellable.Current);

var window = serviceProvider.GetRequiredService<MainWindow>();
app.AddWindow(window);
window.Show();

var mouseState = serviceProvider.GetRequiredService<TileAreaMouseState>();
mouseState.RightClickTile.Subscribe(p => logger.LogDebug("right click on {P}", p));

Gtk.Application.Run();

