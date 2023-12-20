namespace TuringLikePatterns;

internal interface ITickPhase
{
    void RunPhase();
}

internal sealed class GameStateManager(IEnumerable<ITickPhase> phases)
{
    internal event EventHandler<EventArgs>? GameTickPassed;

    private readonly IReadOnlyList<ITickPhase> _phases = phases.ToList();

    internal void Tick()
    {
        foreach (var phase in _phases)
            phase.RunPhase();

        GameTickPassed?.Invoke(null, EventArgs.Empty);
    }
}
