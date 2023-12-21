using Microsoft.Extensions.Logging;
using TuringLikePatterns.Shared.Models;

namespace TuringLikePatterns.Shared.TickPhases;

public sealed class TickIncrementApplier(GameTicker ticker, ILogger<GameTicker> logger)
    : IMutationApplier<TickIncrementMutation>
{
    public void ApplyMutation(TickIncrementMutation mutation)
    {
        var newValue = ticker.TickCount++;
        logger.LogInformation("Ticked to {NewTickerValue}", newValue);
    }
}
