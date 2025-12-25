using System.Collections;
using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
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
    private Connection<TData, TLink>? _connection;
    private IConnection<TData, TLink>? _current;

    protected internal readonly Nerve<TData, TLink> _nerve = args.Nerve;

    public DataLocation<NeuronValue<TData>> Location { get; } = args.Location;
    public long Offset => Location.Offset;

    public ref readonly NeuronValue<TData> RefValue => ref Location.RefValue;
    public ref readonly TData RefData => ref RefValue.Data;

    public void Lock(DataLocationAction<NeuronValue<TData>> action)
    {
        Location.Lock(action);
        CheckCache();
    }

    public async ValueTask LockAsync(DataLocationAsyncAction<NeuronValue<TData>> action,
        CancellationToken cancellationToken = default)
    {
        await Location.LockAsync(action, cancellationToken);
        CheckCache();
    }


    public TResult Lock<TResult>(DataLocationFunc<NeuronValue<TData>, TResult> func)
    {
        var result = Location.Lock(func);
        CheckCache();
        return result;
    }

    public async ValueTask<TResult> LockAsync<TResult>(DataLocationAsyncFunc<NeuronValue<TData>, TResult> func,
        CancellationToken cancellationToken = default)
    {
        var result = await Location.LockAsync(func, cancellationToken);
        CheckCache();
        return result;
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

    public IConnection<TData, TLink>? Find(ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        IConnection<TData, TLink>? previous)
    {
        var neuron = _nerve.RootNeuron.FindOrAdd(data);
        ReadOnlyMemoryValue<NerveCacheKey> cacheKey = NerveHelper.CreateCacheKey(this, neuron, link, previous);
        return FindCore(cacheKey, neuron, link, previous);
    }

    private IConnection<TData, TLink>? FindCore(ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
        INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link,
        IConnection<TData, TLink>? previous)
    {
        if (_nerve.Cache.TryGet<IConnection<TData, TLink>>(in cacheKey.Value, out var connection))
            return connection;

        IConnection<TData, TLink>? result;
        if (previous is not null)
        {
            result = previous.Find(neuron, link);
        }
        else
        {
            result = this.FirstOrDefault(x =>
                x.RefValue.Neuron == neuron.Offset && x.RefLink.Equals(link.Value) && x.RefValue.Previous == -1);

            if (result is not null)
                _nerve.Cache.Set(in cacheKey.Value, result);
        }

        return result;
    }

    public async ValueTask<IConnection<TData, TLink>?> FindAsync(ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        IConnection<TData, TLink>? previous,
        CancellationToken cancellationToken = default)
    {
        var neuron = await _nerve.RootNeuron.FindOrAddAsync(data, cancellationToken);
        ReadOnlyMemoryValue<NerveCacheKey> cacheKey = NerveHelper.CreateCacheKey(this, neuron, link, previous);
        return await FindAsyncCore(cacheKey, neuron, link, previous, cancellationToken);
    }

    private async ValueTask<IConnection<TData, TLink>?> FindAsyncCore(ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
        INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link,
        IConnection<TData, TLink>? previous,
        CancellationToken cancellationToken)
    {
        if (_nerve.Cache.TryGet<IConnection<TData, TLink>>(
                in cacheKey.Value,
                out var connection))
            return connection;

        IConnection<TData, TLink>? result;
        if (previous is not null)
        {
            result = await previous.FindAsync(neuron, link, cancellationToken);
        }
        else
        {
            result = await this.FirstOrDefaultAsync(x =>
                    x.RefValue.Neuron == neuron.Offset && x.RefLink.Equals(link.Value) && x.RefValue.Previous == -1,
                cancellationToken: cancellationToken);

            if (result is not null)
                _nerve.Cache.Set(in cacheKey.Value, result);
        }

        return result;
    }

    public virtual IConnection<TData, TLink> FindOrAdd(ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        IConnection<TData, TLink>? previous)
    {
        var neuron = _nerve.RootNeuron.FindOrAdd(data);
        ReadOnlyMemoryValue<NerveCacheKey> cacheKey = NerveHelper.CreateCacheKey(this, neuron, link, previous);
        return FindCore(cacheKey, neuron, link, previous) ?? AddCore(cacheKey, neuron, link, previous);
    }

    public virtual async ValueTask<IConnection<TData, TLink>> FindOrAddAsync(ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        IConnection<TData, TLink>? previous,
        CancellationToken cancellationToken = default)
    {
        var neuron = await _nerve.RootNeuron.FindOrAddAsync(data, cancellationToken);
        ReadOnlyMemoryValue<NerveCacheKey> cacheKey = NerveHelper.CreateCacheKey(this, neuron, link, previous);
        return await FindAsyncCore(cacheKey, neuron, link, previous, cancellationToken) ??
               await AddAsyncCore(cacheKey, neuron, link, previous, cancellationToken);
    }

    protected virtual IConnection<TData, TLink> AddCore(ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
        INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link,
        IConnection<TData, TLink>? previous)
    {
        if (previous is not null)
        {
            return previous.FindOrAdd(neuron, link);
        }

        return Lock(neuronDataLocation =>
        {
            if (FindCore(cacheKey, neuron, link, null) is { } connection)
            {
                return connection;
            }

            var connectionValue = _nerve.Location.Access
                .Create(ConnectionValue<TLink>.Default with
                {
                    Neuron = neuron.Offset,
                    Next = neuronDataLocation.RefValue.Connection,
                    Link = link.Value,
                });

            neuronDataLocation.RefValue.Connection = connectionValue.Offset;

            return _nerve.Cache.Set(
                in cacheKey.Value,
                _nerve.ConnectionFactory.GetOrCreate(connectionValue.Offset));
        });
    }


    protected virtual async ValueTask<IConnection<TData, TLink>> AddAsyncCore(
        ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
        INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link,
        IConnection<TData, TLink>? previous,
        CancellationToken cancellationToken = default)
    {
        if (previous is not null)
        {
            return await previous.FindOrAddAsync(neuron, link, cancellationToken);
        }

        return await LockAsync(async (neuronDataLocation, neuronCancellationToken) =>
            {
                if (await FindAsyncCore(cacheKey, neuron, link, null, neuronCancellationToken) is { } connection)
                    return connection;

                var connectionValue = await _nerve.Location.Access
                    .CreateAsync(ConnectionValue<TLink>.Default with
                        {
                            Neuron = neuron.Offset,
                            Next = neuronDataLocation.RefValue.Connection,
                            Link = link.Value,
                        },
                        neuronCancellationToken);

                neuronDataLocation.RefValue.Connection = connectionValue.Offset;

                return _nerve.Cache.Set(
                    cacheKey.Value,
                    await _nerve.ConnectionFactory.GetOrCreateAsync(connectionValue.Offset, neuronCancellationToken));
            },
            cancellationToken);
    }

    public virtual IEnumerator<IConnection<TData, TLink>> GetEnumerator()
    {
        _current ??= GetConnection();
        while (_current is not null)
        {
            _nerve.Cache.Set(NerveHelper.CreateCacheKey(this,
                        _current.GetNeuron(),
                        _current.RefLink,
                        _current.GetPrevious())
                    .Value,
                _current);
            yield return _current;
            _current = _current.GetNext();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public async IAsyncEnumerator<IConnection<TData, TLink>> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        _current ??= await GetConnectionAsync(cancellationToken);
        while (_current is not null)
        {
            _nerve.Cache.Set(NerveHelper.CreateCacheKey(this,
                        await _current.GetNeuronAsync(cancellationToken),
                        _current.RefLink,
                        await _current.GetPreviousAsync(cancellationToken))
                    .Value,
                _current);
            yield return _current;
            _current = await _current.GetNextAsync(cancellationToken);
        }
    }
}