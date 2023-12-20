using System.Collections.Generic;
using TuringLikePatterns.TickPhases.Mutations;

namespace TuringLikePatterns.TickPhases.Producers;

public sealed class TickIncrementProducer : IMutationProducer<TickIncrementMutation>
{
    private static readonly TickIncrementMutation[] MutationsSingleton = { new() };

    public IEnumerable<TickIncrementMutation> ProduceMutations() => MutationsSingleton;
}
