using Microsoft.Extensions.DependencyInjection;
using TuringLikePatterns.API;

namespace TuringLikePatterns.GameOfLife;

public class AliveMutationApplier(
    [FromKeyedServices(Constants.GameOfLife)]
    IGameGrid<bool> aliveGrid
) : IMutationApplier<AliveMutation>
{
    public void ApplyMutation(AliveMutation mutation)
    {
        aliveGrid[mutation.Position] = mutation.Alive;
    }
}
