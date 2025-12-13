using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class RootNeuron<TData, TLink>(Neuron<TData, TLink> neuron) : Neuron<TData, TLink>(new(neuron._nerve, neuron.Offset))
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public override IConnection<TData, TLink> GetOrAdd(TData data, TLink link, IConnection<TData, TLink>? connection) =>
        base.GetOrAdd(data, default, null);

    protected override IConnection<TData, TLink> Add(TData data, TLink link, IConnection<TData, TLink>? previous)
    {
        using var scope = _lock.EnterScope();

        if (_cache.TryGetValue(new(data, link, previous?.Offset ?? -1), out var item) && item.IsValueCreated)
            return _nerve.ConnectionFactory.GetOrCreate(item.Value);

        if (Connection?.FirstOrDefault(x =>
                x.Neuron.RefData.Equals(data) && x.RefLink.Equals(link) && x.Previous?.Offset == previous?.Offset) is
            { } connection)
            return connection;

        var neuron = _nerve.NeuronFactory.GetOrCreate(new NerveArgs<TData, TLink>(_nerve,
            _nerve.Location.Access
                .Create(NeuronValue<TData>.Default with { Data = data })
                .Offset));

        var next = RefValue.Connection;
        var connectionItem = _nerve.Location.Access
            .Create(ConnectionValue<TLink>.Default with
            {
                Neuron = neuron.Offset,
                Connection = next,
                Previous = previous?.Offset ?? -1,
                Link = link
            });
        RefValue.Connection = connectionItem.Offset;

        return _nerve.ConnectionFactory.GetOrCreate(connectionItem.Offset);
    }
}