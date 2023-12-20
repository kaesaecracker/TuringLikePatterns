using Microsoft.Extensions.DependencyInjection;
using TuringLikePatterns.Models;
using TuringLikePatterns.TickPhases;

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
