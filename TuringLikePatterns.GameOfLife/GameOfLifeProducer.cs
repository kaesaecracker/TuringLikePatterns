using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Shared.Models;
using TuringLikePatterns.Shared.TickPhases;

namespace TuringLikePatterns.GameOfLife;

internal sealed class GameOfLifeProducer(
    ILogger<GameOfLifeProducer> logger,
    [FromKeyedServices(Constants.GameOfLife)]
    GameGrid<bool> aliveGrid,
    GameBounds bounds,
    ObjectPool<AliveMutation> pool)
    : PoolingMutationProducer<AliveMutation>(pool)
{
    public override IEnumerable<AliveMutation> ProduceMutations()
    {
        for (var x = bounds.TopLeft.Value.X - 1; x <= bounds.BottomRight.Value.X + 1; x++)
        for (var y = bounds.TopLeft.Value.Y - 1; y <= bounds.BottomRight.Value.Y + 1; y++)
        {
            var currentPosition = new GamePosition(x, y);

            var aliveNeighbors = currentPosition.FarNeighborPositions()
                .Count(p => aliveGrid[p]);

            var alive = aliveGrid[currentPosition];
            switch (alive, aliveNeighbors)
            {
                case (true, < 2):
                    logger.LogTrace("{Position} dies because {Neighbors}<2", currentPosition, aliveNeighbors);
                    yield return Pool.GetAliveMutation(currentPosition, false);
                    break;
                case (true, > 3):
                    logger.LogTrace("{Position} dies because {Neighbors}>3", currentPosition, aliveNeighbors);
                    yield return Pool.GetAliveMutation(currentPosition, false);
                    break;
                case (false, 3):
                    logger.LogTrace("{Position} born because {Neighbors}=3", currentPosition, aliveNeighbors);
                    yield return Pool.GetAliveMutation(currentPosition, true);
                    break;
            }
        }
    }
}
