using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class RootNeuron<TData, TLink>(Neuron<TData, TLink> neuron) : Neuron<TData, TLink>(new(neuron._nerve, neuron.Location))
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public override IConnection<TData, TLink> GetOrAdd(TData data, TLink link, IConnection<TData, TLink>? connection) =>
        base.GetOrAdd(data, default, null);

    public override async ValueTask<IConnection<TData, TLink>> GetOrAddAsync(TData data,
        TLink link,
        IConnection<TData, TLink>? connection,
        CancellationToken cancellationToken = default) =>
        await base.GetOrAddAsync(data, default, null, cancellationToken);

    protected override IConnection<TData, TLink> Add(TData data, TLink link, IConnection<TData, TLink>? previous)
    {
        using var scope = _lock.EnterScope();

        var previousOffset = previous?.Offset ?? -1;
        if (_dataCache.TryGetValue(new(data, link, previousOffset), out var connection))
            return connection;

        connection = this.FirstOrDefault(x =>
            x.GetNeuron().RefData.Equals(data) &&
            x.RefLink.Equals(link) &&
            x.RefValue.Previous == previousOffset);

        if (connection is not null)
            return connection;

        var neuronValue = _nerve.Location.Access
            .Create(NeuronValue<TData>.Default with { Data = data });
        var neuron = _nerve.NeuronFactory.GetOrCreate(neuronValue.Offset);

        var connectionValue = _nerve.Location.Access
            .Create(ConnectionValue<TLink>.Default with
            {
                Neuron = neuron.Offset,
                Next = RefValue.Connection,
                Previous = previousOffset,
                Link = link
            });
        connection = _nerve.ConnectionFactory.GetOrCreate(connectionValue.Offset);

        Update(location => location.RefValue.Connection = connectionValue.Offset);

        return connection;
    }

    protected override async ValueTask<IConnection<TData, TLink>> AddAsync(TData data,
        TLink link,
        IConnection<TData, TLink>? previous,
        CancellationToken cancellationToken = default)
    {
        _lock.Enter();
        
        try
        {
            var previousOffset = previous?.Offset ?? -1;
            if (_dataCache.TryGetValue(new(data, link, previousOffset), out var connection))
                return connection;

            connection = await this.FirstOrDefaultAsync(async (x, token) =>
                    (await x.GetNeuronAsync(token)).RefData.Equals(data) &&
                    x.RefLink.Equals(link) &&
                    x.RefValue.Previous == previousOffset,
                cancellationToken);

            if (connection is not null)
                return connection;

            var neuronValue = await _nerve.Location.Access
                .CreateAsync(NeuronValue<TData>.Default with { Data = data }, cancellationToken);
            var neuron = await _nerve.NeuronFactory
                .GetOrCreateAsync(neuronValue.Offset, cancellationToken);

            var connectionValue = await _nerve.Location.Access
                .CreateAsync(ConnectionValue<TLink>.Default with
                    {
                        Neuron = neuron.Offset,
                        Next = RefValue.Connection,
                        Previous = previousOffset,
                        Link = link,
                    },
                    cancellationToken);
            connection = await _nerve.ConnectionFactory.GetOrCreateAsync(connectionValue.Offset, cancellationToken);

            Update(location => location.RefValue.Connection = connectionValue.Offset);

            return connection;
        }
        finally
        {
            _lock.Exit();
        }
    }
}