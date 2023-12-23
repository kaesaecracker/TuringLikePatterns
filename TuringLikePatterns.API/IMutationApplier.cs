namespace TuringLikePatterns.API;

public interface IMutationApplier<in TMut>
    where TMut : IMutation
{
    public void ApplyMutation(TMut mutation);
}
