using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionExtensions
{
    extension<TData, TLink>(Connection<TData, TLink> connection)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public ConnectionWrap<TData, TLink> Wrap(INerve<TData, TLink> nerve) =>
            new(nerve, connection);

        public ConnectionWrap<TData, TLink> Wrap<TWrap>(TWrap wrap)
            where TWrap : ICellWrap<TData, TLink> =>
            new(wrap.Nerve, connection);
    }

    extension<TData, TLink>(ConnectionWrap<TData, TLink> wrap)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public void Lock(DataLocationAction<ConnectionValue<TLink>> action) => wrap.Location.Lock(action);

        public TResult Lock<TResult>(DataLocationFunc<ConnectionValue<TLink>, TResult> func) =>
            wrap.Location.Lock(func);

        public async ValueTask LockAsync(DataLocationAction<ConnectionValue<TLink>> action,
            CancellationToken cancellationToken = default) =>
            await wrap.Location.LockAsync(action, cancellationToken);

        public async ValueTask LockAsync(DataLocationAsyncAction<ConnectionValue<TLink>> action,
            CancellationToken cancellationToken = default) =>
            await wrap.Location.LockAsync(action, cancellationToken);

        public ValueTask<TResult> LockAsync<TResult>(DataLocationFunc<ConnectionValue<TLink>, TResult> func,
            CancellationToken cancellationToken = default) =>
            wrap.Location.LockAsync(func, cancellationToken);

        public ValueTask<TResult> LockAsync<TResult>(DataLocationAsyncFunc<ConnectionValue<TLink>, TResult> func,
            CancellationToken cancellationToken = default) =>
            wrap.Location.LockAsync(func, cancellationToken);

        public Connection<TData, TLink>? Find(in Neuron<TData, TLink> neuron, in TLink link)
        {
            var cacheKey = wrap.Nerve.Cache.CreateConnectionCacheKey(in wrap, in neuron, in link);
            return wrap.FindCore(in cacheKey, in neuron, in link);
        }

        private Connection<TData, TLink>? FindCore(in NerveCacheKey cacheKey,
            in Neuron<TData, TLink> neuron,
            in TLink link)
        {
            if (wrap.Nerve.Cache.TryGetConnectionCacheCore(in cacheKey, out var connectionOffset))
                return new(connectionOffset.Value);

            var localNeuron = neuron;
            var localLink = link;
            var result = wrap.GetUnloadedConnections()
                .FirstOrDefault(x =>
                    x.Wrap(wrap).RefValue.Neuron == localNeuron.Offset && x.Wrap(wrap).RefLink.Equals(localLink))
                .NullWhenDefault();

            if (result.HasValue)
                wrap.Nerve.Cache.TrySetConnectionCacheCore(in cacheKey, result.Value.Offset);

            return result;
        }

        public Connection<TData, TLink> FindOrAdd(in Neuron<TData, TLink> neuron, in TLink link)
        {
            var cacheKey = wrap.Nerve.Cache.CreateConnectionCacheKey(in wrap, in neuron, in link);
            return wrap.FindCore(in cacheKey,in neuron,in link) ?? wrap.AddCore(cacheKey, neuron, link);
        }

        public async ValueTask<Connection<TData, TLink>> FindOrAddAsync(Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = wrap.Nerve.Cache.CreateConnectionCacheKey(in wrap, in neuron, in link.Value);
            return wrap.FindCore(in cacheKey, in neuron, in link.Value) ??
                   await wrap.AddAsyncCore(cacheKey, neuron, link, cancellationToken);
        }

        private Connection<TData, TLink> AddCore(NerveCacheKey cacheKey,
            Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link)
        {
            return wrap.Lock(connectionDataLocation =>
            {
                if (wrap.FindCore(in cacheKey, in neuron, in link.Value) is { } connection)
                    return connection;

                return wrap.NeuronWrap.Lock(neuronDataLocation =>
                {
                    var connectionValue = wrap.Nerve.Access
                        .Create(ConnectionValue<TLink>.Default with
                        {
                            Previous = wrap.Cell.Offset,
                            Neuron = neuron.Offset,
                            Link = link.Value,
                            Next = wrap.NeuronWrap.RefValue.Connection,
                            NextSubConnection = wrap.RefValue.SubConnection
                        });

                    neuronDataLocation.Wrap(wrap.Nerve.Access).RefValue.Connection = connectionValue.Offset;
                    connectionDataLocation.Wrap(wrap.Nerve.Access).RefValue.SubConnection = connectionValue.Offset;

                    wrap.Nerve.Cache.TrySetConnectionCacheCore(in cacheKey, connectionValue.Offset);
                    return new Connection<TData, TLink>(connectionValue.Offset);
                });
            });
        }

        private async ValueTask<Connection<TData, TLink>> AddAsyncCore(
            NerveCacheKey cacheKey,
            Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            return await wrap.LockAsync(async (connectionDataLocation, token) =>
                {
                    if (wrap.FindCore(in cacheKey, in neuron, in link.Value) is { } connection)
                        return connection;

                    return await wrap.NeuronWrap
                        .LockAsync(neuronDataLocation =>
                            {
                                var connectionValue = wrap.Nerve.Access
                                    .Create(ConnectionValue<TLink>.Default with
                                    {
                                        Previous = wrap.Cell.Offset,
                                        Neuron = neuron.Offset,
                                        Link = link.Value,
                                        Next = wrap.NeuronWrap.RefValue.Connection,
                                        NextSubConnection = wrap.RefValue.SubConnection
                                    });

                                neuronDataLocation.Wrap(wrap.Nerve.Access).RefValue.Connection = connectionValue.Offset;
                                connectionDataLocation.Wrap(wrap.Nerve.Access).RefValue.SubConnection = connectionValue.Offset;

                                wrap.Nerve.Cache.TrySetConnectionCacheCore(in cacheKey, connectionValue.Offset);
                                return new Connection<TData, TLink>(connectionValue.Offset);
                            },
                            token);
                },
                cancellationToken);
        }

        public long? LastLoadedConnection
        {
            get => wrap.Nerve.Cache.LastLoadedConnection.GetOrNull(wrap.Cell.Offset);
            set => wrap.Nerve.Cache.LastLoadedConnection.Set(wrap.Cell.Offset, value ?? -1);
        }

        public IEnumerable<Connection<TData, TLink>> GetUnloadedConnections()
        {
            var unloaded = wrap.LastLoadedConnection;
            if (unloaded == -1)
                yield break;

            var connection = unloaded.HasValue ? new Connection<TData, TLink>(unloaded.Value) : wrap.SubConnection;
            while (connection.HasValue)
            {
                var connectionWrap = connection.Value.Wrap(wrap);
                wrap.Nerve.Cache.TrySetConnectionCache(connectionWrap);
                wrap.Nerve.Cache.TrySetNeuronCache(connectionWrap.NeuronWrap);
                wrap.LastLoadedConnection = connectionWrap.NextSubConnection?.Offset;
                connection = connectionWrap.NextSubConnection;
                yield return connectionWrap.Cell;
            }
        }
    }
}