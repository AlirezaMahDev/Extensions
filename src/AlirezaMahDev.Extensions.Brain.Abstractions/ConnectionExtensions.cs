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

        public Connection<TData, TLink>? Find(Neuron<TData, TLink> neuron, ReadOnlyMemoryValue<TLink> link)
        {
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey =
                NerveHelper.CreateCacheKey(wrap.Neuron, neuron, link, wrap.Cell);
            return wrap.FindCore(cacheKey, neuron, link);
        }

        private Connection<TData, TLink>? FindCore(ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
            Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link)
        {
            if (wrap.Nerve.Cache.ConnectionSection.TryGet(in cacheKey.Value,
                    out var connectionOffset))
                return new(connectionOffset.Value);

            var result = wrap.GetUnloadedConnections()
                .FirstOrDefault(x =>
                    x.Wrap(wrap).RefValue.Neuron == neuron.Offset && x.Wrap(wrap).RefLink.Equals(link.Value))
                .NullWhenDefault();

            if (result.HasValue)
                wrap.Nerve.Cache.ConnectionSection.Set(in cacheKey.Value, result.Value.Offset);

            return result;
        }

        public Connection<TData, TLink> FindOrAdd(Neuron<TData, TLink> neuron, ReadOnlyMemoryValue<TLink> link)
        {
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey =
                NerveHelper.CreateCacheKey(wrap.Neuron, neuron, link, wrap.Cell);
            return wrap.FindCore(cacheKey, neuron, link) ?? wrap.AddCore(cacheKey, neuron, link);
        }

        public async ValueTask<Connection<TData, TLink>> FindOrAddAsync(Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey =
                NerveHelper.CreateCacheKey(wrap.Neuron, neuron, link, wrap.Cell);
            return wrap.FindCore(cacheKey, neuron, link) ??
                   await wrap.AddAsyncCore(cacheKey, neuron, link, cancellationToken);
        }

        private Connection<TData, TLink> AddCore(ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
            Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link)
        {
            return wrap.Lock(connectionDataLocation =>
            {
                if (wrap.FindCore(cacheKey, neuron, link) is { } connection)
                    return connection;

                return wrap.Neuron.Wrap(wrap)
                    .Lock(neuronDataLocation =>
                    {
                        var neuronDataLocationWrap = neuronDataLocation.Wrap(wrap.Nerve.Access);
                        var connectionDataLocationWrap = connectionDataLocation.Wrap(wrap.Nerve.Access);
                        var connectionValue = wrap.Nerve.Access
                            .Create(ConnectionValue<TLink>.Default with
                            {
                                Previous = wrap.Cell.Offset,
                                Neuron = neuron.Offset,
                                Next = neuronDataLocationWrap.RefValue.Connection,
                                Link = link.Value,
                                NextSubConnection = connectionDataLocationWrap.RefValue.SubConnection
                            });

                        neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;
                        connectionDataLocationWrap.RefValue.SubConnection = connectionValue.Offset;

                        return new Connection<TData, TLink>(
                            wrap.Nerve.Cache.ConnectionSection.Set(in cacheKey.Value, connectionValue.Offset));
                    });
            });
        }

        private async ValueTask<Connection<TData, TLink>> AddAsyncCore(
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
            Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            return await wrap.LockAsync(async (connectionDataLocation, connectionCancellationToken) =>
                {
                    if (wrap.FindCore(cacheKey, neuron, link) is { } connection)
                        return connection;

                    return await wrap.Neuron.Wrap(wrap)
                        .LockAsync(neuronDataLocation =>
                            {
                                var neuronDataLocationWrap = neuronDataLocation.Wrap(wrap.Nerve.Access);
                                var connectionDataLocationWrap = connectionDataLocation.Wrap(wrap.Nerve.Access);
                                var connectionValue = wrap.Nerve.Access
                                    .Create(ConnectionValue<TLink>.Default with
                                    {
                                        Previous = wrap.Cell.Offset,
                                        Neuron = neuron.Offset,
                                        Next = neuronDataLocationWrap.RefValue.Connection,
                                        Link = link.Value,
                                        NextSubConnection = connectionDataLocationWrap.RefValue.SubConnection
                                    });

                                neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;
                                connectionDataLocationWrap.RefValue.SubConnection = connectionValue.Offset;

                                return new Connection<TData, TLink>(wrap.Nerve.Cache.ConnectionSection.Set(
                                    cacheKey.Value,
                                    connectionValue.Offset));
                            },
                            connectionCancellationToken);
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
                wrap.Nerve.Cache.LastLoadedConnection.Set(NerveHelper.CreateCacheKey(wrap.Neuron,
                            connectionWrap.Neuron,
                            connectionWrap.RefLink,
                            wrap.Cell)
                        .Value,
                    connectionWrap.Cell.Offset);
                yield return connectionWrap.Cell;
                connection = connectionWrap.NextSubConnection;
            }
        }
    }
}