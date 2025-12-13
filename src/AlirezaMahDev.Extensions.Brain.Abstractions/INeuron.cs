using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INeuron<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    long Offset { get; }
    ref NeuronValue<TData> RefValue { get; }
    ref TData RefData { get; }

    IConnection<TData, TLink>? Connection { get; }
    IConnection<TData, TLink> GetOrAdd(TData data, TLink link, IConnection<TData, TLink>? connection);

    IEnumerable<IConnection<TData, TLink>> GetConnections();

    // ValueTask<IConnection<TData, TLink>> GetOrAddAsync(TData data,
    //     TLink link,
    //     IConnection<TData, TLink>? connection,
    //     CancellationToken cancellationToken = default);
}