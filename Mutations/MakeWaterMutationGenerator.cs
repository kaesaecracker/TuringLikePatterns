namespace TuringLikePatterns.Mutations;

internal sealed class MakeWaterMutationGenerator(
    float threshold,
    float portionToReact,
    NamedDataResolver<Quantity> quantities)
    : IMutationGenerator
{
    private readonly Quantity _hydrogen = quantities["hydrogen"];
    private readonly Quantity _water = quantities["water"];
    private readonly Quantity _oxygen = quantities["oxygen"];

    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        foreach (var (position, tile) in state.Tiles)
        {
            var h = tile[_hydrogen];
            if (h < 2 * threshold)
                continue;
            var o = tile[_oxygen];
            if (o < threshold)
                continue;

            h *= portionToReact;
            o *= portionToReact;

            h = Math.Min(h, o / 2f);
            o = Math.Min(o, h * 2f);

            yield return AddQuantityMutation.Get(position, _water, o);
            yield return AddQuantityMutation.Get(position, _oxygen, -o);
            yield return AddQuantityMutation.Get(position, _hydrogen, -h);
        }
    }
}
