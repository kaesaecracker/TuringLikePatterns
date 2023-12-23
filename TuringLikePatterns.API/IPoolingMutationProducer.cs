namespace TuringLikePatterns.API;

public interface IPoolingMutationProducer<TMut> : IMutationProducer<TMut> where TMut : IMutation
{
    void Return(TMut mutation);
}
