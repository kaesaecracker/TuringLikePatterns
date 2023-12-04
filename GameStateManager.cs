using TuringLikePatterns.GameState;
using TuringLikePatterns.Mutations;

namespace TuringLikePatterns;

internal sealed class GameStateManager(
    GameTileField tileField,
    GameTicker ticker,
    IEnumerable<IMutationGenerator> mutationGenerators)
{
    internal event EventHandler<EventArgs>? GameTickPassed;

    private readonly List<IGameStateMutation> _tempMutationList = [];

    internal void Tick()
    {
        _tempMutationList.AddRange(mutationGenerators.SelectMany(mg => mg.GetMutations()));
        foreach (var mutation in _tempMutationList)
        {
            mutation.Apply(tileField, ticker);
            mutation.Dispose();
        }

        _tempMutationList.Clear();

        GameTickPassed?.Invoke(null, EventArgs.Empty);
    }
}
