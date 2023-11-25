using System.Linq;

namespace TuringLikePatterns;

internal sealed record class GameTickPassedEventArgs(GameState OldGameState, GameState NewGameState);

internal sealed class GameStateManager(GameState state, IList<IMutationGenerator> mutationGenerators)
{
    internal event EventHandler<GameTickPassedEventArgs>? GameTickPassed;

    internal void Tick()
    {
        var newState = mutationGenerators.SelectMany(g => g.GetMutations(state))
            .Aggregate(state, (current, mutation) => mutation.Apply(current));

        GameTickPassed?.Invoke(null, new GameTickPassedEventArgs(state, newState));
        state = newState;
    }

    public GameState GetCurrentGameStateDoNotUseThis() => state;
}
