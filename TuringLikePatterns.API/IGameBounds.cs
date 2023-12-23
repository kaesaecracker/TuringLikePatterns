namespace TuringLikePatterns.API;

public interface IGameBounds
{
    GamePosition TopLeft { get; }

    GamePosition BottomRight { get; }

    void ExpandTo(GamePosition pos);
}

