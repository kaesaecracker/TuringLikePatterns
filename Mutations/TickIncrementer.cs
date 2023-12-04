using TuringLikePatterns.GameState;

namespace TuringLikePatterns.Mutations;

internal sealed class TickIncrementerMutationGenerator : IMutationGenerator
{
    private static readonly IGameStateMutation[] MutationsSingleton = { new TickIncrementerMutation() };

    public IEnumerable<IGameStateMutation> GetMutations() => MutationsSingleton;
}

internal sealed class TickIncrementerMutation : IGameStateMutation
{
    public void Apply(GameTileField tileField, GameTicker ticker)
    {
        ticker.TickCount++;
    }
}
