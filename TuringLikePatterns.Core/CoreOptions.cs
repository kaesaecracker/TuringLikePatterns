using TuringLikePatterns.API;

namespace TuringLikePatterns.Core;

public class Quantities
{
    private readonly List<Quantity> _quantities = [];

    internal void Add(Quantity plane)
    {
        _quantities.Add(plane);
    }

    public IReadOnlyList<Quantity> AllQuantities => _quantities;
}


