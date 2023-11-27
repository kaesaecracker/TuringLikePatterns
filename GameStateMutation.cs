using Microsoft.Extensions.ObjectPool;

namespace TuringLikePatterns;

internal interface IGameStateMutation
{
    public void Apply(GameState gameState);
}

internal abstract class PooledGameStateMutation<T> : IGameStateMutation, IDisposable, IResettable
    where T : PooledGameStateMutation<T>, new()
{
    private static readonly ObjectPool<T> Pool = ObjectPool.Create<T>();
    private bool _shouldReturn;

    public void Apply(GameState gameState)
    {
        if (!_shouldReturn)
            throw new InvalidOperationException(
                $"{nameof(PooledGameStateMutation<T>)} not acquired via GetFromPool");
        InnerApply(gameState);
    }

    protected abstract void InnerApply(GameState gameState);

    public bool TryReset() => true;

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
