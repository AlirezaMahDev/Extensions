using System.Numerics;

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

    public void Update(UpdateDataLocationAction<NeuronValue<TData>> action);

    public ValueTask UpdateAsync(UpdateDataLocationAsyncAction<NeuronValue<TData>> action,
        CancellationToken cancellationToken = default);

    IConnection<TData, TLink>? GetConnection();
    ValueTask<IConnection<TData, TLink>?> GetConnectionAsync(CancellationToken cancellationToken = default);

    IConnection<TData, TLink> GetOrAdd(TData data, TLink link, IConnection<TData, TLink>? connection);

    ValueTask<IConnection<TData, TLink>> GetOrAddAsync(TData data,
        TLink link,
        IConnection<TData, TLink>? connection,
        CancellationToken cancellationToken = default);
}