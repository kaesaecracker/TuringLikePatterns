using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TuringLikePatterns.Mutations;

internal sealed class ConwaysGameOfLifeMutationGenerator(
    [FromKeyedServices("Conway's life")] Quantity life,
    ILogger<ConwaysGameOfLifeMutationGenerator> logger
)
    : IMutationGenerator
{
    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        for (var x = state.Tiles.TopLeft.X; x <= state.Tiles.BottomRight.X; x++)
        for (var y = state.Tiles.TopLeft.Y; y <= state.Tiles.BottomRight.Y; y++)
        {
            var currentPosition = new GamePosition(x, y);
            var currentLifeAmount = state.Tiles[currentPosition]?[life] ?? 0f;
            var aliveNeighbors = currentPosition.FarNeighborPositions()
                .Select(p => state.Tiles[p])
                .Count(t => t != null && t[life] >= 1);

            var alive = currentLifeAmount >= 1;
            switch (alive, aliveNeighbors)
            {
                case (true, < 2):
                    logger.LogTrace("{Position} dies because {Neighbors}<2", currentPosition, aliveNeighbors);
                    yield return AddQuantityMutation.Get(currentPosition, life, -currentLifeAmount);
                    break;
                case (true, > 3):
                    logger.LogTrace("{Position} dies because {Neighbors}>3", currentPosition, aliveNeighbors);
                    yield return AddQuantityMutation.Get(currentPosition, life, -currentLifeAmount);
                    break;
                case (false, 3):
                    logger.LogTrace("{Position} born because {Neighbors}=3", currentPosition, aliveNeighbors);
                    yield return AddQuantityMutation.Get(currentPosition, life, 1);
                    break;
                default:
                    logger.LogTrace("{Position} no change for ({Alive}, {Neighbors})", currentPosition, alive,
                        aliveNeighbors);
                    break;
            }
        }
    }
}
