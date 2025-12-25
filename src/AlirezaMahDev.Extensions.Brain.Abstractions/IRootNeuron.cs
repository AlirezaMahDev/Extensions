using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface IRootNeuron<TData, TLink> : IEnumerable<IConnection<TData, TLink>>,
    IAsyncEnumerable<IConnection<TData, TLink>>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    INeuron<TData, TLink>? Find(ReadOnlyMemoryValue<TData> data);
    ValueTask<INeuron<TData, TLink>?> FindAsync(ReadOnlyMemoryValue<TData> data, CancellationToken cancellationToken = default);

    INeuron<TData, TLink> FindOrAdd(ReadOnlyMemoryValue<TData> data);
    ValueTask<INeuron<TData, TLink>> FindOrAddAsync(ReadOnlyMemoryValue<TData> data, CancellationToken cancellationToken = default);
}