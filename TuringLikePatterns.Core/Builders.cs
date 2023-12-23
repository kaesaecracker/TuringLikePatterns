using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TuringLikePatterns.API;
using TuringLikePatterns.Core.Models;

namespace TuringLikePatterns.Core;

public record class TuringLikePatternsBuilder(IServiceCollection Services): ITuringLikePatternsBuilder
{
    public TuringLikePatternsBuilder AddCore()
    {
        Services
            .AddTicking()
            .AddCoreStatistics()
            .AddSingleton<GameBounds>()
            .AddSingleton<GameTileField>()
            .AddSingleton<GameTicker>()
            .AddSingleton<GameStateManager>();
        return this;
    }

    public ITuringLikePatternsBuilder AddPlane(Quantity quantity)
    {
        Services.AddOptions<Quantities>().Configure(quantities => quantities.Add(quantity));
        return this;
    }
}
