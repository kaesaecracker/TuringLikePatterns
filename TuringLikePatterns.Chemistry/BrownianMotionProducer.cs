using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.API;

namespace TuringLikePatterns.Chemistry;

public sealed class BrownianMotionProducer(
    IGameTileField tileField,
    ObjectPool<AddQuantityMutation> pool,
    float threshold,
    float portionToSpread)
    : IPoolingMutationProducer<AddQuantityMutation>
{
    public IEnumerable<AddQuantityMutation> ProduceMutations()
    {
        foreach (var (position, tile) in tileField)
        foreach (var (quantity, currentAmount) in tile)
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
                yield return pool.GetAddQuantity(neighborPos, quantity, amountPerNeighbor);
            }

            yield return pool.GetAddQuantity(position, quantity, -amountToSpread);
        }
    }

    public void Return(IEnumerable<AddQuantityMutation> mutations)
    {
        foreach (var mutation in mutations)
            pool.Return(mutation);
    }

    public void Return(AddQuantityMutation mutation) => pool.Return(mutation);
}
