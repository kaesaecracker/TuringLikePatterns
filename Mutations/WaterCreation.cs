namespace TuringLikePatterns.Mutations;

internal sealed class WaterCreationMutationGenerator : IMutationGenerator
{
    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        foreach (var (_, _) in state.Tiles)
        {
        }

        throw new NotImplementedException();
    }
}
