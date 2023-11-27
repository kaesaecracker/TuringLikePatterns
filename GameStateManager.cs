using System.Collections.Immutable;

namespace TuringLikePatterns;

internal sealed record class GameTickPassedEventArgs(GameState NewGameState);

internal sealed class GameStateManager(GameState state, IList<IMutationGenerator> mutationGenerators)
{
    internal event EventHandler<GameTickPassedEventArgs>? GameTickPassed;

    internal void Tick()
    {
        var mutations = mutationGenerators
            .SelectMany(mg => mg.GetMutations(state))
            .ToImmutableList();
        foreach (var mutation in mutations)
            mutation.Apply(state);

        GameTickPassed?.Invoke(null, new GameTickPassedEventArgs(state));
    }
}

internal interface IMutationGenerator
{
    IEnumerable<IGameStateMutation> GetMutations(GameState state);
}
