using TuringLikePatterns.API;

namespace TuringLikePatterns.Core.TickPhases;

public sealed class TickIncrementProducer : IMutationProducer<TickIncrementMutation>
{
    private static readonly TickIncrementMutation[] MutationsSingleton = { new() };

    public IEnumerable<TickIncrementMutation> ProduceMutations() => MutationsSingleton;
}
