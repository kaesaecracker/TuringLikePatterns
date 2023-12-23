using TuringLikePatterns.API;

namespace TuringLikePatterns.Core.Models;

public sealed class GameBounds: IGameBounds
{
    public void ExpandTo(API.GamePosition pos)
    {
        if (pos.X < TopLeft.X)
            TopLeft = TopLeft with { X = pos.X };
        if (pos.Y < TopLeft.Y)
            TopLeft = TopLeft with { Y = pos.Y };

        if (pos.X > BottomRight.X)
            BottomRight = BottomRight with { X = pos.X };
        if (pos.Y > BottomRight.Y)
            BottomRight = BottomRight with { Y = pos.Y };
    }

    public API.GamePosition TopLeft { get; private set; }

    public API.GamePosition BottomRight { get; private set; }
}
