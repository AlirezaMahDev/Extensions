using System.Collections.Concurrent;
using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class NerveCache<TData, TLink> : INerveCache<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    private readonly ConcurrentDictionary<string, NerveCacheSection> _cache = [];

    public INerveCacheSection GetOrAdd(string key) =>
        _cache.GetOrAdd(key, static _ => new());
}