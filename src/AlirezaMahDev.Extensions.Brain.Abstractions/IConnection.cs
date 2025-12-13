using System.Numerics;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface IConnection<TData, TLink> : IEnumerable<IConnection<TData, TLink>>,
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
    ref ConnectionValue<TLink> RefValue { get; }
    ref TLink RefLink { get; }
    INeuron<TData, TLink> Neuron { get; }
    IConnection<TData, TLink>? Previous { get; }
    IConnection<TData, TLink>? Next { get; }
    IConnection<TData, TLink>[] ToArray();
}