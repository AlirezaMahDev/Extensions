using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface IConnection<TData, TLink> : IEnumerable<IConnection<TData, TLink>>,
    IAsyncEnumerable<IConnection<TData, TLink>>,
    IComparable<DataPairLink<TData, TLink>>,
    IComparable<TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    DataLocation<ConnectionValue<TLink>> Location { get; }

    long Offset { get; }
    ref readonly ConnectionValue<TLink> RefValue { get; }
    ref readonly TLink RefLink { get; }

    public void Lock(DataLocationAction<ConnectionValue<TLink>> action);

    public ValueTask LockAsync(DataLocationAsyncAction<ConnectionValue<TLink>> action,
        CancellationToken cancellationToken = default);

    public TResult Lock<TResult>(DataLocationFunc<ConnectionValue<TLink>, TResult> func);

    public ValueTask<TResult> LockAsync<TResult>(DataLocationAsyncFunc<ConnectionValue<TLink>, TResult> func,
        CancellationToken cancellationToken = default);

    INeuron<TData, TLink> GetNeuron();
    ValueTask<INeuron<TData, TLink>> GetNeuronAsync(CancellationToken cancellationToken = default);

    IConnection<TData, TLink>? GetPrevious();
    ValueTask<IConnection<TData, TLink>?> GetPreviousAsync(CancellationToken cancellationToken = default);

    IConnection<TData, TLink>? GetNext();
    ValueTask<IConnection<TData, TLink>?> GetNextAsync(CancellationToken cancellationToken = default);

    IConnection<TData, TLink>? GetSubConnection();
    ValueTask<IConnection<TData, TLink>?> GetSubConnectionAsync(CancellationToken cancellationToken = default);

    IConnection<TData, TLink>? GetNextSubConnection();
    ValueTask<IConnection<TData, TLink>?> GetNextSubConnectionAsync(CancellationToken cancellationToken = default);

    IConnection<TData, TLink>? Find(INeuron<TData, TLink> neuron, ReadOnlyMemoryValue<TLink> link);

    ValueTask<IConnection<TData, TLink>?> FindAsync(INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link,
        CancellationToken cancellationToken = default);

    IConnection<TData, TLink> FindOrAdd(INeuron<TData, TLink> neuron, ReadOnlyMemoryValue<TLink> link);

    ValueTask<IConnection<TData, TLink>> FindOrAddAsync(INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link,
        CancellationToken cancellationToken = default);
}