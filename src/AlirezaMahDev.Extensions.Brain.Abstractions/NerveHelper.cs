using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

static class NerveHelper
{
    public static ReadOnlyMemoryValue<NerveCacheKey> CreateCacheKey<TData>(in ReadOnlyMemoryValue<TData> data)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData> =>
        NerveCacheKey.Create(in data.Value);

    public static ReadOnlyMemoryValue<NerveCacheKey> CreateCacheKey<TData, TLink>(
            in Neuron<TData, TLink> from,
            in Neuron<TData, TLink> to,
            in ReadOnlyMemoryValue<TLink> link,
            in Connection<TData, TLink>? connection)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink> =>
        NerveCacheKey.Create(from.Offset, to.Offset, in link.Value, connection?.Offset ?? -1);
}