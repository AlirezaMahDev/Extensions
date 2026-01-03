using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerveCache<TData, TLink>
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
{
    INerveCacheSection GetOrAdd(string key);
}