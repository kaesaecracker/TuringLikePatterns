namespace TuringLikePatterns.TickPhases;

internal sealed class TickPhase<TProducer, TMut, TApplier>(TProducer producer, TApplier applier) : ITickPhase
    where TMut : Mutation
    where TProducer : IMutationProducer<TMut>
    where TApplier : IMutationApplier<TMut>
{
    private readonly List<TMut> _tempMutationList = [];

    public void RunPhase()
    {
        _tempMutationList.AddRange(producer.ProduceMutations());

        foreach (var mutation in _tempMutationList)
            applier.ApplyMutation(mutation);

        if (producer is PoolingMutationProducer<TMut> poolingProducer)
            poolingProducer.ReturnToPool(_tempMutationList);

        _tempMutationList.Clear();
    }
}
