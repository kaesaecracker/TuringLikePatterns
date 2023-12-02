using System.Collections.Immutable;

namespace TuringLikePatterns;

internal sealed class NamedDataResolver<T>
    where T : NamedData
{
    private readonly ImmutableDictionary<string, T> _dict;

    public NamedDataResolver(IEnumerable<T> allInstances)
    {
        _dict = allInstances.ToImmutableDictionary(i => i.Name);
    }

    public T this[string name] => _dict[name];
}
