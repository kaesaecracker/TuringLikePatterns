using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.API;

namespace TuringLikePatterns.GameOfLife;

public sealed record class AliveMutation : IMutation, IResettable
{
    public bool TryReset() => true;

    public bool Alive { get; set; }

    public GamePosition Position { get; set; }
}
