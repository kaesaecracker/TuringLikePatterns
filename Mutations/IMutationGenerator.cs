namespace TuringLikePatterns.Mutations;

internal interface IMutationGenerator
{
    IEnumerable<IGameStateMutation> GetMutations();
}
