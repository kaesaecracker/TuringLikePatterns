namespace TuringLikePatterns.Mutations;

internal sealed class ManualMutationQueue : IMutationGenerator
{
    private readonly Queue<IGameStateMutation> _queuedMutations = new();

    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        while (_queuedMutations.TryDequeue(out var result))
            yield return result;
    }

    public void Enqueue(IGameStateMutation mutation) => _queuedMutations.Enqueue(mutation);
}
