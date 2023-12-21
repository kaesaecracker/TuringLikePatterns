using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Core.Models;
using TuringLikePatterns.Core.TickPhases;

namespace TuringLikePatterns.GameOfLife;

public sealed record class AliveMutation : Mutation, IResettable
{
    public bool TryReset() => true;

    public bool Alive { get; set; }

    public GamePosition Position { get; set; }
}
