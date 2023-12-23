using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.API;

namespace TuringLikePatterns.GameOfLife;

internal sealed class GameOfLifeProducer(
    ILogger<GameOfLifeProducer> logger,
    [FromKeyedServices(Constants.GameOfLife)]
    IGameGrid<bool> aliveGrid,
    IGameBounds bounds,
    ObjectPool<AliveMutation> pool)
    : IPoolingMutationProducer<AliveMutation>
{
    public IEnumerable<AliveMutation> ProduceMutations()
    {
        for (var x = bounds.TopLeft.X - 1; x <= bounds.BottomRight.X + 1; x++)
        for (var y = bounds.TopLeft.Y - 1; y <= bounds.BottomRight.Y + 1; y++)
        {
            var currentPosition = new GamePosition(x, y);

            var aliveNeighbors = currentPosition.FarNeighborPositions()
                .Count(p => aliveGrid[p]);

            var alive = aliveGrid[currentPosition];
            switch (alive, aliveNeighbors)
            {
                case (true, < 2):
                    logger.LogTrace("{Position} dies because {Neighbors}<2", currentPosition, aliveNeighbors);
                    yield return pool.GetAliveMutation(currentPosition, false);
                    break;
                case (true, > 3):
                    logger.LogTrace("{Position} dies because {Neighbors}>3", currentPosition, aliveNeighbors);
                    yield return pool.GetAliveMutation(currentPosition, false);
                    break;
                case (false, 3):
                    logger.LogTrace("{Position} born because {Neighbors}=3", currentPosition, aliveNeighbors);
                    yield return pool.GetAliveMutation(currentPosition, true);
                    break;
            }
        }
    }

    public void Return(AliveMutation mutation) => pool.Return(mutation);
}
