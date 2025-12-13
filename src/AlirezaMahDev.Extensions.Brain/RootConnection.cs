using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class RootConnection<TData, TLink>(Connection<TData, TLink> connection)
    : Connection<TData, TLink>(new(connection._nerve, connection.Offset))
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public override INeuron<TData, TLink> Neuron { get; } = new RootNeuron<TData, TLink>(connection._neuron);
}