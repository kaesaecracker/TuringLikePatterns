namespace TuringLikePatterns.Mutations;

internal sealed class BrownianMotionMutationGenerator : IMutationGenerator
{
    public IEnumerable<IGameStateMutation> GetMutations(GameState state)
    {
        foreach (var (pos, tile) in state.Tiles)
        {
            if (tile.Hydrogen > 1)
            {
                var amount = tile.Hydrogen / 100;
                yield return new AddHydrogenMutation(pos, -amount);
                yield return new AddHydrogenMutation(pos.Left(), amount / 4);
                yield return new AddHydrogenMutation(pos.Top(), amount / 4);
                yield return new AddHydrogenMutation(pos.Right(), amount / 4);
                yield return new AddHydrogenMutation(pos.Bottom(), amount / 4);
            }

            if (tile.Oxygen > 1)
            {
                var amount = tile.Oxygen / 100;
                yield return new AddOxygenMutation(pos, -amount);
                yield return new AddOxygenMutation(pos.Left(), amount / 4);
                yield return new AddOxygenMutation(pos.Top(), amount / 4);
                yield return new AddOxygenMutation(pos.Right(), amount / 4);
                yield return new AddOxygenMutation(pos.Bottom(), amount / 4);
            }

            if (tile.Water > 1)
            {
                var amount = tile.Water / 100;
                yield return new AddWaterMutation(pos, -amount);
                yield return new AddWaterMutation(pos.Left(), amount / 4);
                yield return new AddWaterMutation(pos.Top(), amount / 4);
                yield return new AddWaterMutation(pos.Right(), amount / 4);
                yield return new AddWaterMutation(pos.Bottom(), amount / 4);
            }
        }
    }
}
