using GLib;
using Microsoft.Extensions.DependencyInjection;
using TuringLikePatterns;
using TuringLikePatterns.Gui;
using TuringLikePatterns.Mutations;

var serviceProvider = ConfigureServices();
Gtk.Application.Init();

var app = serviceProvider.GetRequiredService<GtkApplication>();
app.Register(Cancellable.Current);

var window = serviceProvider.GetRequiredService<MainWindow>();
app.AddWindow(window);
window.Show();

Gtk.Application.Run();
return;

static ServiceProvider ConfigureServices()
{
    var services = new ServiceCollection();

    // Data
    services.AddSingleton(new Quantity("water", SKColors.Aqua));
    services.AddSingleton(new Quantity("hydrogen", SKColors.Blue));
    services.AddSingleton(new Quantity("oxygen", SKColors.White));
    services.AddSingleton<NamedDataResolver<Quantity>>();
    return new ServiceCollection()
        // Data
        .AddQuantity(new Quantity("water", SKColors.Aqua))
        .AddQuantity(new Quantity("hydrogen", SKColors.Blue))
        .AddQuantity(new Quantity("oxygen", SKColors.White))

        // Logic
        .AddMutationGenerator<TickIncrementerMutationGenerator>()
        .AddMutationGenerator<BrownianMotionMutationGenerator>(1f, 0.01f)
        .AddMutationGenerator<MakeWaterMutationGenerator>( /*threshold*/1f, /*portionToReact*/0.333f)
        .AddMutationGenerator<ManualMutationQueue>()

        // State
        .AddSingleton<GameStateManager>()

        // GUI
        .AddSingleton<MainWindow>()
        .AddSingleton<GtkApplication>()
        .AddSingleton<TileDrawingArea>()
        .AddSingleton<ActionsPage>()
        .AddSingleton<StatisticsPage>()

        // Done!
        .BuildServiceProvider();
}
