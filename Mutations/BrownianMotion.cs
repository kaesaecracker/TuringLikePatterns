namespace TuringLikePatterns.Mutations;

internal sealed class BrownianMotionMutationGenerator(float threshold, float portionToSpread) : IMutationGenerator
{
    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        foreach (var (position, tile) in state.Tiles)
        foreach (var (quantity, amount) in tile)
        {
            if (amount <= threshold)
                continue;

            var amountToSpread = amount * portionToSpread;

            yield return AddQuantityMutation.Get(position, quantity, -amountToSpread);

            var amountForNeighbors = amountToSpread / 4;
            yield return AddQuantityMutation.Get(position.Left(), quantity, amountForNeighbors);
            yield return AddQuantityMutation.Get(position.Top(), quantity, amountForNeighbors);
            yield return AddQuantityMutation.Get(position.Right(), quantity, amountForNeighbors);
            yield return AddQuantityMutation.Get(position.Bottom(), quantity, amountForNeighbors);
        }
    }
}
