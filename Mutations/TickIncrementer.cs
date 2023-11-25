namespace TuringLikePatterns.Mutations;

internal sealed class TickIncrementerMutationGenerator : IMutationGenerator
{
    private readonly IGameStateMutation[] _mutationsSingleton = { new TickIncrementerMutation() };

    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        return _mutationsSingleton;
    }
}

internal sealed class TickIncrementerMutation : IGameStateMutation
{
    public GameState Apply(GameState gameState)
    {
        return gameState with { TickCount = gameState.TickCount + 1 };
    }
}
