using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Models;
using TuringLikePatterns.TickPhases;
using TuringLikePatterns.TickPhases.Mutations;

namespace TuringLikePatterns.GameOfLife;

internal sealed class GameOfLifeProducer(
    [FromKeyedServices("Conway's life")] Quantity life,
    ILogger<GameOfLifeProducer> logger,
    GameTileField tileField,
    GameBounds bounds,
    ObjectPool<AddQuantityMutation> pool)
    : PoolingMutationProducer<AddQuantityMutation>(pool)
{
    public override IEnumerable<AddQuantityMutation> ProduceMutations()
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
                    yield return Pool.GetAddQuantity(currentPosition, life, -currentLifeAmount);
                    break;
                case (true, > 3):
                    logger.LogTrace("{Position} dies because {Neighbors}>3", currentPosition, aliveNeighbors);
                    yield return Pool.GetAddQuantity(currentPosition, life, -currentLifeAmount);
                    break;
                case (false, 3):
                    logger.LogTrace("{Position} born because {Neighbors}=3", currentPosition, aliveNeighbors);
                    yield return Pool.GetAddQuantity(currentPosition, life, 1);
                    break;
            }
        }
    }
}
