using System.Collections.Generic;

namespace TuringLikePatterns.Shared.TickPhases;

public abstract record class Mutation;

public interface IMutationProducer<out TMut>
    where TMut : Mutation
{
    IEnumerable<TMut> ProduceMutations();
}

public interface IMutationApplier<in TMut>
{
    public void ApplyMutation(TMut mutation);
}
