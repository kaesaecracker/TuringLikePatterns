namespace TuringLikePatterns.Mutations;

internal sealed class MakeWaterMutationGenerator(float threshold, float portionToReact) : IMutationGenerator
{
    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        foreach (var (position, tile) in state.Tiles)
        {
            var h = tile[Quantity.Hydrogen];
            if (h < 2 * threshold)
                continue;
            var o = tile[Quantity.Oxygen];
            if (o < threshold)
                continue;

            h *= portionToReact;
            o *= portionToReact;

            h = Math.Min(h, o / 2f);
            o = Math.Min(o, h * 2f);

            yield return AddQuantityMutation.Get(position, Quantity.Water, o);
            yield return AddQuantityMutation.Get(position, Quantity.Oxygen, -o);
            yield return AddQuantityMutation.Get(position, Quantity.Hydrogen, -h);
        }
    }
}
