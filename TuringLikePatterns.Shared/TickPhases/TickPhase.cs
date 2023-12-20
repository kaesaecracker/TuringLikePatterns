using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace TuringLikePatterns.TickPhases;

internal sealed class TickPhase<TProducer, TMut, TApplier>(
    TProducer producer,
    TApplier applier,
    ILogger<TickPhase<TProducer, TMut, TApplier>> logger)
    : ITickPhase
    where TMut : Mutation
    where TProducer : IMutationProducer<TMut>
    where TApplier : IMutationApplier<TMut>
{
    private readonly List<TMut> _tempMutationList = [];

    public void RunPhase()
    {
        _tempMutationList.AddRange(producer.ProduceMutations());

        foreach (var mutation in _tempMutationList)
        {
            logger.LogTrace("applying {Mutation}", mutation);
            applier.ApplyMutation(mutation);
        }

        if (producer is PoolingMutationProducer<TMut> poolingProducer)
            poolingProducer.ReturnToPool(_tempMutationList);

        _tempMutationList.Clear();
    }
}
