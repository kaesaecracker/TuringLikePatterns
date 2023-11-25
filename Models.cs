namespace TuringLikePatterns;


internal interface IMutationGenerator
{
    IEnumerable<IGameStateMutation> GetMutations(GameState state);
}

internal interface IGameStateMutation
{
    GameState Apply(GameState gameState);
}
