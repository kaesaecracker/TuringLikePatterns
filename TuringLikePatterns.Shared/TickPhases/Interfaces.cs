using System.Collections.Generic;

namespace TuringLikePatterns.TickPhases;

public abstract record class Mutation;

public interface IMutationProducer<out TMut>
    where TMut : Mutation
{
    IEnumerable<TMut> ProduceMutations();
}

public interface IMutationApplier<in TMut>
    where TMut : Mutation
{
    public void ApplyMutation(TMut mutation);
}
