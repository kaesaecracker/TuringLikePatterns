namespace TuringLikePatterns.TickPhases;

internal abstract record class Mutation;

internal interface IMutationProducer<out TMut>
    where TMut : Mutation
{
    IEnumerable<TMut> ProduceMutations();
}

internal interface IMutationApplier<in TMut>
    where TMut : Mutation
{
    public void ApplyMutation(TMut mutation);
}
