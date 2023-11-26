namespace TuringLikePatterns.Mutations;

internal sealed class TickIncrementerMutationGenerator : IMutationGenerator
{
    private static readonly IGameStateMutation[] MutationsSingleton = { new TickIncrementerMutation() };

    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        return MutationsSingleton;
    }
}

internal sealed class TickIncrementerMutation : IGameStateMutation
{
    public GameState Apply(GameState gameState)
    {
        return gameState with { TickCount = gameState.TickCount + 1 };
    }
}
