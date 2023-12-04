namespace TuringLikePatterns.GameState;

internal readonly record struct GamePosition(long X, long Y)
{
    internal GamePosition Top() => this with { Y = Y - 1 };

    internal GamePosition Bottom() => this with { Y = Y + 1 };

    internal GamePosition Left() => this with { X = X - 1 };

    internal GamePosition Right() => this with { X = X + 1 };

    internal IEnumerable<GamePosition> FarNeighborPositions()
    {
        var temp = Top();
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

    internal IEnumerable<GamePosition> NearNeighborPositions()
    {
        yield return Top();
        yield return Right();
        yield return Bottom();
        yield return Left();
    }

    public override string ToString() => $"({X} | {Y})";
}
