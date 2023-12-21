using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Core;
using TuringLikePatterns.Core.Models;
using TuringLikePatterns.Core.TickPhases;

namespace TuringLikePatterns.Chemistry;

public sealed class MakeWaterProducer(
    [FromKeyedServices(nameof(hydrogen))] Quantity hydrogen,
    [FromKeyedServices(nameof(water))] Quantity water,
    [FromKeyedServices(nameof(oxygen))] Quantity oxygen,
    GameTileField tileField,
    ObjectPool<AddQuantityMutation> pool,
    float threshold,
    float portionToReact
)
    : PoolingMutationProducer<AddQuantityMutation>(pool)
{
    public override IEnumerable<AddQuantityMutation> ProduceMutations()
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

            yield return Pool.GetAddQuantity(position, water, o);
            yield return Pool.GetAddQuantity(position, oxygen, -o);
            yield return Pool.GetAddQuantity(position, hydrogen, -h);
        }
    }
}
