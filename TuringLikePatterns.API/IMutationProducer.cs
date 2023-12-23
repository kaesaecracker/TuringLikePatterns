namespace TuringLikePatterns.API;

public interface IMutationProducer<out TMut>
    where TMut : IMutation
{
    IEnumerable<TMut> ProduceMutations();
}
