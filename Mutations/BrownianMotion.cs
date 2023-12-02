namespace TuringLikePatterns.Mutations;

internal sealed class BrownianMotionMutationGenerator(float threshold, float portionToSpread) : IMutationGenerator
{
    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        foreach (var (position, tile) in state.Tiles)
        foreach (var (quantity, currentAmount) in tile.Raw)
        {
            if (currentAmount <= threshold)
                continue;

            var amountPerNeighbor = currentAmount * portionToSpread / 4;
            var amountToSpread = 0f;

            foreach (var neighborPos in position.Neighbors())
            {
                // TODO: this mutates tile.Raw and thus does not work
                //if (state.Tiles[neighborPos][quantity] > currentAmount)
                //    continue;

                amountToSpread += amountPerNeighbor;
                yield return AddQuantityMutation.Get(neighborPos, quantity, amountPerNeighbor);
            }

            yield return AddQuantityMutation.Get(position, quantity, -amountToSpread);
        }
    }
}
