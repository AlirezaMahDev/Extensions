using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NeuronWrapExtensions
{
    extension<TData, TLink>(NeuronWrap<TData, TLink> wrap)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public void Lock(DataLocationAction<NeuronValue<TData>> action) =>
            wrap.Location.Lock(action);

        public TResult Lock<TResult>(DataLocationFunc<NeuronValue<TData>, TResult> func) =>
            wrap.Location.Lock(func);

        public async ValueTask LockAsync(DataLocationAction<NeuronValue<TData>> action,
            CancellationToken cancellationToken = default) =>
            await wrap.Location.LockAsync(action, cancellationToken);

        public async ValueTask LockAsync(DataLocationAsyncAction<NeuronValue<TData>> action,
            CancellationToken cancellationToken = default) =>
            await wrap.Location.LockAsync(action, cancellationToken);

        public async ValueTask<TResult> LockAsync<TResult>(DataLocationFunc<NeuronValue<TData>, TResult> func,
            CancellationToken cancellationToken = default) =>
            await wrap.Location.LockAsync(func, cancellationToken);

        public async ValueTask<TResult> LockAsync<TResult>(DataLocationAsyncFunc<NeuronValue<TData>, TResult> func,
            CancellationToken cancellationToken = default) =>
            await wrap.Location.LockAsync(func, cancellationToken);

        public Connection<TData, TLink>? Find(in TData data, in TLink link)
        {
            var neuron = wrap.Nerve.FindOrAddNeuron(data);
            var cacheKey = wrap.Nerve.Cache.CreateNeuronConnectionCacheKey(wrap.Cell, in neuron, in link);
            return wrap.FindCore(in cacheKey, neuron, link);
        }

        private Connection<TData, TLink>? FindCore(in NerveCacheKey cacheKey,
            in Neuron<TData, TLink> neuron,
            in TLink link)
        {
            if (wrap.Nerve.Cache.TryGetNeuronConnectionCacheCore(in cacheKey, out var connectionOffset))
                return new(connectionOffset.Value);

            var localNeuron = neuron;
            var localLink = link;
            var result = wrap.GetUnloadedConnections()
                .Select(x => x.Wrap(wrap))
                .FirstOrDefault(x =>
                    x.RefValue.Neuron == localNeuron.Offset &&
                    x.RefLink.Equals(localLink) &&
                    x.RefValue.Previous.IsNull)
                .NullWhenDefault()
                ?.Cell;

            if (result.HasValue)
                wrap.Nerve.Cache.SetNeuronConnectionCacheCore(in cacheKey, result.Value.Offset);

            return result;
        }

        public Connection<TData, TLink> FindOrAdd(ReadOnlyMemoryValue<TData> data,
            ReadOnlyMemoryValue<TLink> link)
        {
            var neuron = wrap.Nerve.FindOrAddNeuron(data);
            var cacheKey = wrap.Nerve.Cache.CreateNeuronConnectionCacheKey(wrap.Cell, in neuron, in link.Value);
            return wrap.FindCore(in cacheKey, in neuron, in link.Value) ?? wrap.AddCore(cacheKey, neuron, link);
        }

        public async ValueTask<Connection<TData, TLink>> FindOrAddAsync(ReadOnlyMemoryValue<TData> data,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            var neuron = await wrap.Nerve.FindOrAddNeuronAsync(data, cancellationToken);
            var cacheKey = wrap.Nerve.Cache.CreateNeuronConnectionCacheKey(wrap.Cell, in neuron, in link.Value);
            return wrap.FindCore(in cacheKey, in neuron, in link.Value) ??
                   await wrap.AddAsyncCore(cacheKey, neuron, link, cancellationToken);
        }


        private Connection<TData, TLink> AddCore(NerveCacheKey cacheKey,
            Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link)
        {
            return wrap.Lock(neuronDataLocation =>
            {
                var neuronDataLocationWrap = neuronDataLocation.Wrap(wrap.Nerve.Access);
                if (wrap.FindCore(in cacheKey, in neuron, in link.Value) is { } connection)
                {
                    return connection;
                }

                var connectionValue = wrap.Nerve.Access
                    .Create(ConnectionValue<TLink>.Default with
                    {
                        Neuron = neuron.Offset,
                        Next = neuronDataLocationWrap.RefValue.Connection,
                        Link = link.Value,
                    });

                neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;

                wrap.Nerve.Cache.TrySetNeuronConnectionCacheCore(in cacheKey, connectionValue.Offset);
                return new(connectionValue.Offset);
            });
        }


        private async ValueTask<Connection<TData, TLink>> AddAsyncCore(
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
            Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            return await wrap.LockAsync(neuronDataLocation =>
                {
                    if (wrap.FindCore(in cacheKey.Value, in neuron, in link.Value) is { } connection)
                        return connection;

                    var neuronDataLocationWrap = neuronDataLocation.Wrap(wrap.Nerve.Access);
                    var connectionValue = wrap.Nerve.Access
                        .Create(ConnectionValue<TLink>.Default with
                        {
                            Neuron = neuron.Offset,
                            Next = neuronDataLocationWrap.RefValue.Connection,
                            Link = link.Value,
                        });


                    neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;

                    wrap.Nerve.Cache.TrySetNeuronConnectionCacheCore(in cacheKey.Value, connectionValue.Offset);
                    return new(connectionValue.Offset);
                },
                cancellationToken);
        }

        public DataOffset? LastLoadedConnection
        {
            get => wrap.Nerve.Cache.LastLoadedConnection.GetOrNull(wrap.Cell.Offset);
            set => wrap.Nerve.Cache.LastLoadedConnection.Set(wrap.Cell.Offset, value ?? DataOffset.Null);
        }

        public IEnumerable<Connection<TData, TLink>> GetUnloadedConnections()
        {
            var unloaded = wrap.LastLoadedConnection;
            if (unloaded?.IsNull == true)
                yield break;

            var connection = unloaded.HasValue ? new Connection<TData, TLink>(unloaded.Value) : wrap.Connection;
            while (connection.HasValue)
            {
                var connectionWrap = connection.Value.Wrap(wrap);
                wrap.Nerve.Cache.TrySetNeuronConnectionCache(wrap.Cell, connectionWrap);
                wrap.Nerve.Cache.TrySetNeuronCache(connectionWrap.NeuronWrap);
                wrap.LastLoadedConnection = connectionWrap.Next?.Offset;
                connection = connectionWrap.Next;
                yield return connectionWrap.Cell;
            }
        }


        public IEnumerable<ConnectionWrap<TData, TLink>> GetUnloadedConnectionsWrap() =>
            wrap.GetUnloadedConnections().Select(x => x.Wrap(wrap));
    }
}