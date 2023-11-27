namespace TuringLikePatterns.Mutations;

internal sealed class TickIncrementerMutationGenerator : IMutationGenerator
{
    private static readonly IGameStateMutation[] MutationsSingleton = { new TickIncrementerMutation() };

    public IEnumerable<IGameStateMutation> GetMutations(GameState state) => MutationsSingleton;
}

internal sealed class TickIncrementerMutation : IGameStateMutation
{
    public void Apply(GameState gameState)
    {
        gameState.TickCount++;
    }
}
