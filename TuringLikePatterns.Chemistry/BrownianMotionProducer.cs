using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Shared.Models;
using TuringLikePatterns.Shared.TickPhases.Mutations;

namespace TuringLikePatterns.Shared.TickPhases.Producers;

public sealed class BrownianMotionProducer(
    GameTileField tileField,
    ObjectPool<AddQuantityMutation> pool,
    float threshold,
    float portionToSpread)
    : PoolingMutationProducer<AddQuantityMutation>(pool)
{
    public override IEnumerable<AddQuantityMutation> ProduceMutations()
    {
        foreach (var (position, tile) in tileField)
        foreach (var (quantity, currentAmount) in tile.Raw)
        {
            if (currentAmount < threshold)
                continue;

            var amountPerNeighbor = currentAmount * portionToSpread / 4;
            var amountToSpread = 0f;

            foreach (var neighborPos in position.NearNeighborPositions())
            {
                // TODO: this mutates tile.Raw and thus does not work
                //if (state.Tiles[neighborPos][quantity] > currentAmount)
                //    continue;

                amountToSpread += amountPerNeighbor;
                yield return Pool.GetAddQuantity(neighborPos, quantity, amountPerNeighbor);
            }

            yield return Pool.GetAddQuantity(position, quantity, -amountToSpread);
        }
    }
}
