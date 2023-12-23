namespace TuringLikePatterns.API;

public static class GamePositionExtensions
{
    public static GamePosition Top(this GamePosition pos) => pos with { Y = pos.Y - 1 };

    public  static GamePosition Bottom(this GamePosition pos) => pos with { Y = pos.Y + 1 };

    public  static GamePosition Left(this GamePosition pos) => pos with { X = pos.X - 1 };

    public  static GamePosition Right(this GamePosition pos) => pos with { X = pos.X + 1 };

    public static IEnumerable<GamePosition> FarNeighborPositions(this GamePosition pos)
    {
        var temp = pos.Top();
        yield return temp;
        temp = temp.Right();
        yield return temp;
        temp = temp.Bottom();
        yield return temp;
        temp = temp.Bottom();
        yield return temp;
        temp = temp.Left();
        yield return temp;
        temp = temp.Left();
        yield return temp;
        temp = temp.Top();
        yield return temp;
        temp = temp.Top();
        yield return temp;
    }

    public static IEnumerable<GamePosition> NearNeighborPositions(this GamePosition pos)
    {
        yield return pos.Top();
        yield return pos.Right();
        yield return pos.Bottom();
        yield return pos.Left();
    }
}
