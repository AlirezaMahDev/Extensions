using System.Collections;
using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;


namespace AlirezaMahDev.Extensions.Brain;

class Connection<TData, TLink>(ConnectionArgs<TData, TLink> args) : IConnection<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    protected internal readonly Nerve<TData, TLink> _nerve = args.Nerve;

    private Neuron<TData, TLink>? _neuron;

    private Connection<TData, TLink>? _previous;
    private Connection<TData, TLink>? _next;
    private Connection<TData, TLink>? _subConnection;
    private Connection<TData, TLink>? _nextSubConnection;

    private IConnection<TData, TLink>? _current;

    public virtual DataLocation<ConnectionValue<TLink>> Location { get; } = args.Location;

    public long Offset => Location.Offset;
    public ref readonly ConnectionValue<TLink> RefValue => ref Location.RefValue;

    public ref readonly TLink RefLink => ref RefValue.Link;

    public void Lock(DataLocationAction<ConnectionValue<TLink>> action)
    {
        Location.Lock(action);
        CheckCache();
    }

    public async ValueTask LockAsync(DataLocationAsyncAction<ConnectionValue<TLink>> action,
        CancellationToken cancellationToken = default)
    {
        await Location.LockAsync(action, cancellationToken);
        CheckCache();
    }

    public TResult Lock<TResult>(DataLocationFunc<ConnectionValue<TLink>, TResult> func)
    {
        var result = Location.Lock(func);
        CheckCache();
        return result;
    }

    public ValueTask<TResult> LockAsync<TResult>(DataLocationAsyncFunc<ConnectionValue<TLink>, TResult> func,
        CancellationToken cancellationToken = default)
    {
        var result = Location.LockAsync(func, cancellationToken);
        CheckCache();
        return result;
    }

    private void CheckCache()
    {
        if ((_previous?.Offset ?? -1L) != RefValue.Previous)
            _previous = null;
        if ((_next?.Offset ?? -1L) != RefValue.Next)
            _next = null;
        if ((_subConnection?.Offset ?? -1L) != RefValue.SubConnection)
            _subConnection = null;
        if ((_nextSubConnection?.Offset ?? -1L) != RefValue.NextSubConnection)
            _nextSubConnection = null;
    }

    public virtual INeuron<TData, TLink> GetNeuron() =>
        _neuron ??= _nerve.NeuronFactory.GetOrCreate(RefValue.Neuron);

    public virtual async ValueTask<INeuron<TData, TLink>>
        GetNeuronAsync(CancellationToken cancellationToken = default) =>
        _neuron ??= await _nerve.NeuronFactory.GetOrCreateAsync(RefValue.Neuron, cancellationToken);

    public IConnection<TData, TLink>? GetPrevious() =>
        _previous ??= RefValue.Previous != -1
            ? _nerve.ConnectionFactory.GetOrCreate(RefValue.Previous)
            : null;

    public async ValueTask<IConnection<TData, TLink>?>
        GetPreviousAsync(CancellationToken cancellationToken = default) =>
        _previous ??= RefValue.Previous != -1
            ? await _nerve.ConnectionFactory.GetOrCreateAsync(RefValue.Previous, cancellationToken)
            : null;

    public IConnection<TData, TLink>? GetNext() =>
        _next ??= RefValue.Next != -1
            ? _nerve.ConnectionFactory.GetOrCreate(RefValue.Next)
            : null;

    public async ValueTask<IConnection<TData, TLink>?> GetNextAsync(CancellationToken cancellationToken = default) =>
        _next ??= RefValue.Next != -1
            ? await _nerve.ConnectionFactory.GetOrCreateAsync(RefValue.Next, cancellationToken)
            : null;

    public IConnection<TData, TLink>? GetSubConnection() =>
        _subConnection ??= RefValue.SubConnection != -1
            ? _nerve.ConnectionFactory.GetOrCreate(RefValue.SubConnection)
            : null;

    public async ValueTask<IConnection<TData, TLink>?> GetSubConnectionAsync(CancellationToken cancellationToken =
        default) =>
        _subConnection ??= RefValue.SubConnection != -1
            ? await _nerve.ConnectionFactory.GetOrCreateAsync(RefValue.SubConnection, cancellationToken)
            : null;

    public IConnection<TData, TLink>? GetNextSubConnection() =>
        _nextSubConnection ??= RefValue.NextSubConnection != -1
            ? _nerve.ConnectionFactory.GetOrCreate(RefValue.NextSubConnection)
            : null;

    public async ValueTask<IConnection<TData, TLink>?> GetNextSubConnectionAsync(CancellationToken cancellationToken =
        default) =>
        _nextSubConnection ??= RefValue.NextSubConnection != -1
            ? await _nerve.ConnectionFactory.GetOrCreateAsync(RefValue.NextSubConnection, cancellationToken)
            : null;

    public IConnection<TData, TLink>? Find(INeuron<TData, TLink> neuron, ReadOnlyMemoryValue<TLink> link)
    {
        ReadOnlyMemoryValue<NerveCacheKey> cacheKey = NerveHelper.CreateCacheKey(GetNeuron(), neuron, link, this);
        return FindCore(cacheKey, neuron, link);
    }

    private IConnection<TData, TLink>? FindCore(ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
        INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link)
    {
        if (_nerve.Cache.TryGet<IConnection<TData, TLink>>(in cacheKey.Value, out var connection))
            return connection;

        var result = this.FirstOrDefault(x =>
            x.RefValue.Neuron == neuron.Offset && x.RefLink.Equals(link.Value));

        if (result is not null)
            _nerve.Cache.Set(in cacheKey.Value, result);

        return result;
    }

    public async ValueTask<IConnection<TData, TLink>?> FindAsync(INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link,
        CancellationToken cancellationToken = default)
    {
        ReadOnlyMemoryValue<NerveCacheKey> cacheKey =
            NerveHelper.CreateCacheKey(await GetNeuronAsync(cancellationToken), neuron, link, this);
        return await FindAsyncCore(cacheKey, neuron, link, cancellationToken);
    }

    private async ValueTask<IConnection<TData, TLink>?> FindAsyncCore(ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
        INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link,
        CancellationToken cancellationToken = default)
    {
        if (_nerve.Cache.TryGet<IConnection<TData, TLink>>(in cacheKey.Value, out var connection))
            return connection;

        var result = await this.FirstOrDefaultAsync(x =>
                x.RefValue.Neuron == neuron.Offset && x.RefLink.Equals(link.Value),
            cancellationToken: cancellationToken);

        if (result is not null)
            _nerve.Cache.Set(in cacheKey.Value, result);

        return result;
    }

    public IConnection<TData, TLink> FindOrAdd(INeuron<TData, TLink> neuron, ReadOnlyMemoryValue<TLink> link)
    {
        ReadOnlyMemoryValue<NerveCacheKey> cacheKey = NerveHelper.CreateCacheKey(GetNeuron(), neuron, link, this);
        return FindCore(cacheKey, neuron, link) ?? AddCore(cacheKey, neuron, link);
    }

    public async ValueTask<IConnection<TData, TLink>> FindOrAddAsync(INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link,
        CancellationToken cancellationToken = default)
    {
        ReadOnlyMemoryValue<NerveCacheKey> cacheKey =
            NerveHelper.CreateCacheKey(await GetNeuronAsync(cancellationToken), neuron, link, this);
        return await FindAsyncCore(cacheKey, neuron, link, cancellationToken) ??
               await AddAsyncCore(cacheKey, neuron, link, cancellationToken);
    }

    protected virtual IConnection<TData, TLink> AddCore(ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
        INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link)
    {
        return Lock(connectionDataLocation =>
        {
            if (FindCore(cacheKey, neuron, link) is { } connection)
                return connection;

            return GetNeuron()
                .Lock(neuronDataLocation =>
                {
                    var connectionValue = _nerve.Location.Access
                        .Create(ConnectionValue<TLink>.Default with
                        {
                            Previous = Offset,
                            Neuron = neuron.Offset,
                            Next = neuronDataLocation.RefValue.Connection,
                            Link = link.Value,
                            NextSubConnection = connectionDataLocation.RefValue.SubConnection
                        });

                    neuronDataLocation.RefValue.Connection = connectionValue.Offset;
                    connectionDataLocation.RefValue.SubConnection = connectionValue.Offset;

                    return _nerve.Cache.Set(
                        in cacheKey.Value,
                        _nerve.ConnectionFactory.GetOrCreate(connectionValue.Offset));
                });
        });
    }

    protected virtual async ValueTask<IConnection<TData, TLink>> AddAsyncCore(
        ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
        INeuron<TData, TLink> neuron,
        ReadOnlyMemoryValue<TLink> link,
        CancellationToken cancellationToken = default)
    {
        return await LockAsync(async (connectionDataLocation, connectionCancellationToken) =>
            {
                if (await FindAsyncCore(cacheKey, neuron, link, connectionCancellationToken) is { } connection)
                    return connection;

                return await (await GetNeuronAsync(connectionCancellationToken))
                    .LockAsync(async (neuronDataLocation, neuronCancellationToken) =>
                        {
                            var connectionValue = _nerve.Location.Access
                                .Create(ConnectionValue<TLink>.Default with
                                {
                                    Previous = Offset,
                                    Neuron = neuron.Offset,
                                    Next = neuronDataLocation.RefValue.Connection,
                                    Link = link.Value,
                                    NextSubConnection = connectionDataLocation.RefValue.SubConnection
                                });

                            neuronDataLocation.RefValue.Connection = connectionValue.Offset;
                            connectionDataLocation.RefValue.SubConnection = connectionValue.Offset;

                            return _nerve.Cache.Set(
                                cacheKey.Value,
                                await _nerve.ConnectionFactory.GetOrCreateAsync(connectionValue.Offset,
                                    neuronCancellationToken));
                        },
                        connectionCancellationToken);
            },
            cancellationToken);
    }

    public int CompareTo(DataPairLink<TData, TLink> other)
    {
        var link = Math.Abs(Comparer<TLink>.Default.Compare(RefLink, other.Link));
        if (link != 0)
            return link;

        var data = Math.Abs(Comparer<TData>.Default.Compare(GetNeuron().RefData, other.Data));
        if (data != 0)
            return data;

        return 0;
    }

    public int CompareTo(TLink other)
    {
        var link = Math.Abs(Comparer<TLink>.Default.Compare(RefLink, other));
        if (link != 0)
            return link;

        return 0;
    }

    public virtual IEnumerator<IConnection<TData, TLink>> GetEnumerator()
    {
        _current ??= GetSubConnection();
        while (_current is not null)
        {
            _nerve.Cache.Set(NerveHelper.CreateCacheKey(
                        GetNeuron(),
                        _current.GetNeuron(),
                        _current.RefLink,
                        this)
                    .Value,
                _current);
            yield return _current;
            _current = _current.GetNextSubConnection();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public async IAsyncEnumerator<IConnection<TData, TLink>>
        GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        _current ??= await GetSubConnectionAsync(cancellationToken);
        while (_current is not null)
        {
            _nerve.Cache.Set(NerveHelper.CreateCacheKey(
                        await GetNeuronAsync(cancellationToken),
                        await _current.GetNeuronAsync(cancellationToken),
                        _current.RefLink,
                        this)
                    .Value,
                _current);
            yield return _current;
            _current = await _current.GetNextSubConnectionAsync(cancellationToken);
        }
    }
}