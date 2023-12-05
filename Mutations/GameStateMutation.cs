using Microsoft.Extensions.ObjectPool;
using TuringLikePatterns.Models;

namespace TuringLikePatterns.Mutations;

internal interface IGameStateMutation : IDisposable
{
    public void Apply(GameTileField tileField, GameTicker ticker);

    void IDisposable.Dispose()
    {
    }
}

internal abstract class PooledGameStateMutation<T> : IGameStateMutation, IResettable
    where T : PooledGameStateMutation<T>, new()
{
    private static readonly ObjectPool<T> Pool =
        new DefaultObjectPool<T>(new DefaultPooledObjectPolicy<T>(), int.MaxValue);

    private bool _shouldReturn;

    public void Apply(GameTileField tileField, GameTicker ticker)
    {
        if (!_shouldReturn)
            throw new InvalidOperationException(
                $"{nameof(PooledGameStateMutation<T>)} not acquired via GetFromPool");
        InnerApply(tileField, ticker);
    }

    protected abstract void InnerApply(GameTileField tileField, GameTicker ticker);

    public bool TryReset()
    {
        _shouldReturn = false;
        return true;
    }

    protected static T GetFromPool()
    {
        var result = Pool.Get();
        result._shouldReturn = true;
        return result;
    }

    public void Dispose()
    {
        if (!_shouldReturn)
            return;

        _shouldReturn = false;
        Pool.Return((T)this);
    }
}
