using Microsoft.Extensions.ObjectPool;

namespace TuringLikePatterns;

internal interface IGameStateMutation
{
    public GameState Apply(GameState gameState);
}

internal abstract class PooledGameStateMutation<T> : IGameStateMutation, IDisposable, IResettable
    where T : PooledGameStateMutation<T>, new()
{
    private static readonly ObjectPool<T> Pool = ObjectPool.Create<T>();
    private bool _shouldReturn;

    public virtual GameState Apply(GameState gameState)
    {
        if (!_shouldReturn)
            throw new InvalidOperationException(
                $"{nameof(PooledGameStateMutation<T>)} not acquired via GetFromPool");

        return gameState;
    }

    public virtual bool TryReset()
    {
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
