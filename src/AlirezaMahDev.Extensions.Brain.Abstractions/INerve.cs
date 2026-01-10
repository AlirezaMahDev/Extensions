using System.Numerics;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerve<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    INerveCache<TData, TLink> Cache { get; }
    IDataAccess Access { get; }

    string Name { get; }
    DataLocation<DataPath> Location { get; }
    DataLocation<DataPath> ConnectionLocation { get; }
    DataLocation<DataPath> NeuronLocation { get; }

    Neuron<TData, TLink> Neuron { get; }
    NeuronWrap<TData, TLink> NeuronWrap { get; }
    Connection<TData, TLink> Connection { get; }
    ConnectionWrap<TData, TLink> ConnectionWrap { get; }

    public void Flush();
}