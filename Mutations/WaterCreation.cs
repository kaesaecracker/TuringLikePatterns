namespace TuringLikePatterns.Mutations;

internal sealed class WaterCreationMutationGenerator : IMutationGenerator
{
    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        foreach (var (pos, tile) in state.Tiles)
        {
        }

        throw new NotImplementedException();
    }
}
