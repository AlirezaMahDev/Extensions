using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INeuron<TData, TLink> : IEnumerable<IConnection<TData, TLink>>,
    IAsyncEnumerable<IConnection<TData, TLink>>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    DataLocation<NeuronValue<TData>> Location { get; }

    long Offset { get; }

    ref readonly NeuronValue<TData> RefValue { get; }
    ref readonly TData RefData { get; }

    public void Lock(DataLocationAction<NeuronValue<TData>> action);

    public ValueTask LockAsync(DataLocationAsyncAction<NeuronValue<TData>> action,
        CancellationToken cancellationToken = default);

    public TResult Lock<TResult>(DataLocationFunc<NeuronValue<TData>, TResult> func);

    public ValueTask<TResult> LockAsync<TResult>(DataLocationAsyncFunc<NeuronValue<TData>, TResult> func,
        CancellationToken cancellationToken = default);

    IConnection<TData, TLink>? GetConnection();
    ValueTask<IConnection<TData, TLink>?> GetConnectionAsync(CancellationToken cancellationToken = default);

    IConnection<TData, TLink>? Find(ReadOnlyMemoryValue<TData> data, ReadOnlyMemoryValue<TLink> link, IConnection<TData, TLink>? connection);

    ValueTask<IConnection<TData, TLink>?> FindAsync(ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        IConnection<TData, TLink>? connection,
        CancellationToken cancellationToken = default);

    IConnection<TData, TLink> FindOrAdd(ReadOnlyMemoryValue<TData> data, ReadOnlyMemoryValue<TLink> link, IConnection<TData, TLink>? connection);

    ValueTask<IConnection<TData, TLink>> FindOrAddAsync(ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        IConnection<TData, TLink>? connection,
        CancellationToken cancellationToken = default);
}