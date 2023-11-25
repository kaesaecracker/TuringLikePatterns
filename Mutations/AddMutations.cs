namespace TuringLikePatterns.Mutations;

internal sealed record class AddHydrogenMutation(GamePosition Position, float Amount) : IGameStateMutation
{
    public GameState Apply(GameState gameState)
    {
        var newTiles = new Dictionary<GamePosition, GameTile>(gameState.Tiles);
        var oldState = gameState.Tiles[Position];
        newTiles[Position] = oldState with { Hydrogen = oldState.Hydrogen + Amount };
        return gameState with { Tiles = new GameStateTiles(newTiles) };
    }
}

internal sealed record class AddOxygenMutation(GamePosition Position, float Amount) : IGameStateMutation
{
    public GameState Apply(GameState gameState)
    {
        var newTiles = new Dictionary<GamePosition, GameTile>(gameState.Tiles);
        var oldState = gameState.Tiles[Position];
        newTiles[Position] = oldState with { Oxygen = oldState.Oxygen + Amount };
        return gameState with { Tiles = new GameStateTiles(newTiles) };
    }
}

internal sealed record class AddWaterMutation(GamePosition Position, float Amount) : IGameStateMutation
{
    public GameState Apply(GameState gameState)
    {
        var newTiles = new Dictionary<GamePosition, GameTile>(gameState.Tiles);
        var oldState = gameState.Tiles[Position];
        newTiles[Position] = oldState with { Water = oldState.Water + Amount };
        return gameState with { Tiles = new GameStateTiles(newTiles) };
    }
}
