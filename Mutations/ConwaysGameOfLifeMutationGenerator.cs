using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuringLikePatterns.GameState;

namespace TuringLikePatterns.Mutations;

internal sealed class ConwaysGameOfLifeMutationGenerator(
    [FromKeyedServices("Conway's life")] Quantity life,
    ILogger<ConwaysGameOfLifeMutationGenerator> logger,
    GameTileField tileField,
    GameBounds bounds
)
    : IMutationGenerator
{
    public IEnumerable<IGameStateMutation> GetMutations()
    {
        for (var x = bounds.TopLeft.Value.X - 1; x <= bounds.BottomRight.Value.X + 1; x++)
        for (var y = bounds.TopLeft.Value.Y - 1; y <= bounds.BottomRight.Value.Y + 1; y++)
        {
            var currentPosition = new GamePosition(x, y);
            var currentLifeAmount = tileField[currentPosition]?[life] ?? 0f;
            var aliveNeighbors = currentPosition.FarNeighborPositions()
                .Select(p => tileField[p])
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
