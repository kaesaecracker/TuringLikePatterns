namespace TuringLikePatterns;

internal sealed record class GameTickPassedEventArgs(GameState OldGameState, GameState NewGameState);

internal sealed class GameStateManager(GameState state, IList<IMutationGenerator> mutationGenerators)
{
    internal event EventHandler<GameTickPassedEventArgs>? GameTickPassed;

    internal void Tick()
    {
        var newState = state;

        foreach (var g in mutationGenerators)
        foreach (var mutation in g.GetMutations(state))
            newState = mutation.Apply(newState);

        GameTickPassed?.Invoke(null, new GameTickPassedEventArgs(state, newState));
        state = newState;
    }
}

internal interface IMutationGenerator
{
    IEnumerable<IGameStateMutation> GetMutations(GameState state);
}
