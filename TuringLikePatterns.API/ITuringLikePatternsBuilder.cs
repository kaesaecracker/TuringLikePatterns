using Microsoft.Extensions.DependencyInjection;

namespace TuringLikePatterns.API;

public interface ITuringLikePatternsBuilder
{
    IServiceCollection Services { get; }

    ITuringLikePatternsBuilder AddPlane(Quantity quantity);
}
