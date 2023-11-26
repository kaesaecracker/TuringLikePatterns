using Microsoft.Extensions.ObjectPool;

namespace TuringLikePatterns;

internal sealed class AutoPoolingDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDisposable, IResettable
    where TKey : notnull
{
    private static readonly ObjectPool<AutoPoolingDictionary<TKey, TValue>> Pool =
        ObjectPool.Create<AutoPoolingDictionary<TKey, TValue>>();

    private bool _shouldReturn;

    public static AutoPoolingDictionary<TKey, TValue> GetFromPool(
        IReadOnlyDictionary<TKey, TValue>? initialItems = null)
    {
        var result = Pool.Get();
        result._shouldReturn = true;

        if (initialItems == null)
            return result;

        result.EnsureCapacity(initialItems.Count);
        foreach (var (key, value) in initialItems)
            result.Add(key, value);
        return result;
    }

    public void Dispose()
    {
        if (!_shouldReturn)
            return;
        _shouldReturn = false;
        Pool.Return(this);
    }

    public bool TryReset()
    {
        Clear();
        return true;
    }
}
