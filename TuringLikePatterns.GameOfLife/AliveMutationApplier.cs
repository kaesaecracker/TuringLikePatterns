using Microsoft.Extensions.DependencyInjection;
using TuringLikePatterns.Core.Models;
using TuringLikePatterns.Core.TickPhases;

namespace TuringLikePatterns.GameOfLife;

public class AliveMutationApplier(
    [FromKeyedServices(Constants.GameOfLife)]
    GameGrid<bool> aliveGrid
) : IMutationApplier<AliveMutation>
{
    public void ApplyMutation(AliveMutation mutation)
    {
        aliveGrid[mutation.Position] = mutation.Alive;
    }
}
