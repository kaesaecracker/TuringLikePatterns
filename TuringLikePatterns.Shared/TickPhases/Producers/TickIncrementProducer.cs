using System.Collections.Generic;
using TuringLikePatterns.Shared.TickPhases.Mutations;

namespace TuringLikePatterns.Shared.TickPhases.Producers;

public sealed class TickIncrementProducer : IMutationProducer<TickIncrementMutation>
{
    private static readonly TickIncrementMutation[] MutationsSingleton = { new() };

    public IEnumerable<TickIncrementMutation> ProduceMutations() => MutationsSingleton;
}
