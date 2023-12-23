using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.API;

namespace TuringLikePatterns.Chemistry;

public sealed class MakeWaterProducer(
    [FromKeyedServices(nameof(hydrogen))] Quantity hydrogen,
    [FromKeyedServices(nameof(water))] Quantity water,
    [FromKeyedServices(nameof(oxygen))] Quantity oxygen,
    IGameTileField tileField,
    ObjectPool<AddQuantityMutation> pool,
    float threshold,
    float portionToReact
)
    : IPoolingMutationProducer<AddQuantityMutation>
{
    public IEnumerable<AddQuantityMutation> ProduceMutations()
    {
        foreach (var (position, tile) in tileField)
        {
            var h = tile[hydrogen];
            if (h < 2 * threshold)
                continue;
            var o = tile[oxygen];
            if (o < threshold)
                continue;

            h *= portionToReact;
            o *= portionToReact;

            h = Math.Min(h, o / 2f);
            o = Math.Min(o, h * 2f);

            yield return pool.GetAddQuantity(position, water, o);
            yield return pool.GetAddQuantity(position, oxygen, -o);
            yield return pool.GetAddQuantity(position, hydrogen, -h);
        }
    }

    public void Return(AddQuantityMutation mutation) => pool.Return(mutation);
}
