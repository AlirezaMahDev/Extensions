using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class RootConnection<TData, TLink>(Connection<TData, TLink> connection)
    : Connection<TData, TLink>(new(connection._nerve, connection.Location))
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    private RootNeuron<TData, TLink>? _rootNeuron;

    public override INeuron<TData, TLink> GetNeuron()
    {
        _ = connection.GetNeuron();
        _rootNeuron ??= new(connection._neuron!);
        return _rootNeuron;
    }

    public override async ValueTask<INeuron<TData, TLink>> GetNeuronAsync(CancellationToken cancellationToken = default)
    {
        _ = await connection.GetNeuronAsync(cancellationToken);
        _rootNeuron ??= new(connection._neuron!);
        return _rootNeuron;
    }
}