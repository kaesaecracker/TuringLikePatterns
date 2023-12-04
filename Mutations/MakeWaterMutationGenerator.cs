using Microsoft.Extensions.DependencyInjection;
using TuringLikePatterns.GameState;

namespace TuringLikePatterns.Mutations;

internal sealed class MakeWaterMutationGenerator(
    float threshold,
    float portionToReact,
    [FromKeyedServices(nameof(hydrogen))] Quantity hydrogen,
    [FromKeyedServices(nameof(water))] Quantity water,
    [FromKeyedServices(nameof(oxygen))] Quantity oxygen,
    GameTileField tileField
)
    : IMutationGenerator
{
    public IEnumerable<IGameStateMutation> GetMutations()
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

            yield return AddQuantityMutation.Get(position, water, o);
            yield return AddQuantityMutation.Get(position, oxygen, -o);
            yield return AddQuantityMutation.Get(position, hydrogen, -h);
        }
    }
}
