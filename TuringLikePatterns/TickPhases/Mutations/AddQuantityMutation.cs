using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Models;

namespace TuringLikePatterns.TickPhases.Mutations;

internal sealed record class AddQuantityMutation : Mutation, IResettable
{
    private Quantity? _quantityToChange;

    public float Amount { get; private set; }

    public GamePosition Position { get; private set; }

    public Quantity QuantityToChange
    {
        get => _quantityToChange ?? throw new InvalidOperationException();
        private set => _quantityToChange = value;
    }

    public void SetValues(GamePosition position, Quantity quantityToChange, float amount)
    {
        ArgumentOutOfRangeException.ThrowIfZero(amount);

        Position = position;
        QuantityToChange = quantityToChange;
        Amount = amount;
    }

    public bool TryReset() => true;
}
