namespace TuringLikePatterns;

internal sealed class GameStateManager(IEnumerable<IMutationGenerator> mutationGenerators)
{
    internal event EventHandler<EventArgs>? GameTickPassed;

    internal GameState State { get; } = new();

    private readonly List<IGameStateMutation> _tempMutationList = [];

    internal void Tick()
    {
        _tempMutationList.AddRange(mutationGenerators.SelectMany(mg => mg.GetMutations(State)));
        foreach (var mutation in _tempMutationList)
        {
            mutation.Apply(State);
            mutation.Dispose();
        }

        _tempMutationList.Clear();

        GameTickPassed?.Invoke(null, EventArgs.Empty);
    }
}

internal interface IMutationGenerator
{
    IEnumerable<IGameStateMutation> GetMutations(GameState state);
}
