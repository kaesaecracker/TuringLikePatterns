using System.Linq;

namespace TuringLikePatterns.Core;

public interface ITickPhase
{
    void RunPhase();
}

public sealed class GameStateManager(IEnumerable<ITickPhase> phases)
{
    public event EventHandler<EventArgs>? GameTickPassed;

    private readonly IReadOnlyList<ITickPhase> _phases = phases.ToList();

    public void Tick()
    {
        foreach (var phase in _phases)
            phase.RunPhase();

        GameTickPassed?.Invoke(null, EventArgs.Empty);
    }
}
