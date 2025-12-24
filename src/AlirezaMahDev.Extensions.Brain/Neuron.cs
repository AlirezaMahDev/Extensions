using System.Collections;
using System.Collections.Concurrent;
using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class Neuron<TData, TLink>(NeuronArgs<TData, TLink> args) : INeuron<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    protected readonly ConcurrentDictionary<NeuronCacheKey<TData, TLink>, IConnection<TData, TLink>> _dataCache = [];
    private Connection<TData, TLink>? _connection;

    protected readonly Lock _lock = new();
    protected internal readonly Nerve<TData, TLink> _nerve = args.Nerve;

    public DataLocation<NeuronValue<TData>> Location { get; } = args.Location;
    public long Offset => Location.Offset;

    public ref readonly NeuronValue<TData> RefValue => ref Location.RefValue;
    public ref readonly TData RefData => ref RefValue.Data;

    public void Update(UpdateDataLocationAction<NeuronValue<TData>> action)
    {
        Location.Update(action);
        CheckCache();
    }

    public async ValueTask UpdateAsync(UpdateDataLocationAsyncAction<NeuronValue<TData>> action,
        CancellationToken cancellationToken = default)
    {
        await Location.UpdateAsync(action, cancellationToken);
        CheckCache();
    }

    private void CheckCache()
    {
        if ((_connection?.Offset ?? -1L) != RefValue.Connection)
            _connection = null;
    }

    public IConnection<TData, TLink>? GetConnection() =>
        _connection ??= RefValue.Connection != -1
            ? _nerve.ConnectionFactory.GetOrCreate(RefValue.Connection)
            : null;

    public async ValueTask<IConnection<TData, TLink>?>
        GetConnectionAsync(CancellationToken cancellationToken = default) =>
        _connection ??= RefValue.Connection != -1
            ? await _nerve.ConnectionFactory.GetOrCreateAsync(RefValue.Connection, cancellationToken)
            : null;

    public virtual IConnection<TData, TLink> GetOrAdd(TData data, TLink link, IConnection<TData, TLink>? connection) =>
        _dataCache.GetOrAdd(new(data, link, connection?.Offset ?? -1),
            static (key, args) =>
                args.neuron.Add(key.Data, key.Link, args.connection),
            (neuron: this, connection));

    public virtual async ValueTask<IConnection<TData, TLink>> GetOrAddAsync(TData data,
        TLink link,
        IConnection<TData, TLink>? connection,
        CancellationToken cancellationToken = default) =>
        _dataCache.TryGetValue(new(data, link, connection?.Offset ?? -1), out var connectionItem)
            ? connectionItem
            : await AddAsync(data, link, connection, cancellationToken);

    protected virtual IConnection<TData, TLink> Add(TData data, TLink link, IConnection<TData, TLink>? previous)
    {
        using var scope = _lock.EnterScope();

        var previousOffset = previous?.Offset ?? -1;
        if (_dataCache.TryGetValue(new(data, link, previousOffset), out var connection))
            return connection;

        if (previous is not null)
            connection = previous.FirstOrDefault(x =>
                x.GetNeuron().RefData.Equals(data) &&
                x.RefLink.Equals(link));

        if (connection is not null)
            return connection;

        connection = _nerve.RootNeuron.GetOrAdd(data, link, previous);

        var connectionValue = _nerve.Location.Access
            .Create(ConnectionValue<TLink>.Default with
            {
                Neuron = connection.RefValue.Neuron,
                Next = RefValue.Connection,
                Previous = previousOffset,
                Link = link,
                NextSubConnection = previous?.RefValue.SubConnection ?? -1
            });
        connection = _nerve.ConnectionFactory.GetOrCreate(connectionValue.Offset);

        Update(location => location.RefValue.Connection = connectionValue.Offset);
        previous?.Update(location => location.RefValue.SubConnection = connectionValue.Offset);

        return connection;
    }


    protected virtual async ValueTask<IConnection<TData, TLink>> AddAsync(TData data,
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

            if (previous is not null)
                connection = await previous.FirstOrDefaultAsync(async (x, token) =>
                        (await x.GetNeuronAsync(token)).RefData.Equals(data) &&
                        x.RefLink.Equals(link),
                    cancellationToken);

            if (connection is not null)
                return connection;

            connection = await _nerve.RootNeuron.GetOrAddAsync(data, link, previous, cancellationToken);

            var next = RefValue.Connection;
            var connectionValue = await _nerve.Location.Access
                .CreateAsync(ConnectionValue<TLink>.Default with
                    {
                        Neuron = connection.RefValue.Neuron,
                        Next = next,
                        Previous = previousOffset,
                        Link = link,
                        NextSubConnection = previous?.RefValue.SubConnection ?? -1
                    },
                    cancellationToken);
            connection = await _nerve.ConnectionFactory.GetOrCreateAsync(connectionValue.Offset, cancellationToken);

            Update(location => location.RefValue.Connection = connectionValue.Offset);
            previous?.Update(location => location.RefValue.SubConnection = connectionValue.Offset);

            return connection;
        }
        finally
        {
            _lock.Exit();
        }
    }

    public virtual IEnumerator<IConnection<TData, TLink>> GetEnumerator()
    {
        var current = GetConnection();
        while (current is not null)
        {
            _ = _dataCache.GetOrAdd(
                new(current.GetNeuron().RefData, current.RefLink, current.RefValue.Previous),
                current);
            yield return current;
            current = current.GetNext();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public async IAsyncEnumerator<IConnection<TData, TLink>> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        var current = await GetConnectionAsync(cancellationToken);
        while (current is not null)
        {
            _ = _dataCache.GetOrAdd(
                new((await current.GetNeuronAsync(cancellationToken)).RefData,
                    current.RefLink,
                    current.RefValue.Previous),
                current);
            yield return current;
            current = await current.GetNextAsync(cancellationToken);
        }
    }
}