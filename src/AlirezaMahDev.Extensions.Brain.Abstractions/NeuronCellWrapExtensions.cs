using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NeuronCellWrapExtensions
{
    extension<TData, TLink>(CellWrap<Neuron, NeuronValue<TData>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public ref readonly TData RefData => ref wrap.Location.RefValue.Data;

        public Connection? Connection =>
            wrap.RefValue.Connection.IsNull
                ? null
                : new(wrap.RefValue.Connection);

        public CellWrap<Connection,ConnectionValue<TLink>,TData, TLink>? ConnectionWrap =>
            wrap.Connection?.Wrap(wrap.Nerve);

        public IEnumerable<Connection> GetConnections()
        {
            var current = wrap.Connection;
            while (current.HasValue)
            {
                yield return current.Value;
                current = current.Value.Wrap(wrap.Nerve).Next;
            }
        }

        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap()
        {
            var current = wrap.ConnectionWrap;
            while (current.HasValue)
            {
                yield return current.Value;
                current = current.Value.NextWrap;
            }
        }

        public Connection? Find(in TData data, in TLink link)
        {
            var neuron = wrap.Nerve.FindOrAddNeuron(data);
            var cacheKey = wrap.Nerve.CreateNeuronConnectionCacheKey(wrap.Cell, in neuron, in link);
            return wrap.FindCore(in cacheKey, neuron, link);
        }

        private Connection? FindCore(in NerveCacheKey cacheKey,
            in Neuron neuron,
            in TLink link)
        {
            if (wrap.Nerve.TryGetNeuronConnectionCacheCore(in cacheKey, out var connectionOffset))
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
                wrap.Nerve.SetNeuronConnectionCacheCore(in cacheKey, result.Value.Offset);

            return result;
        }

        public Connection FindOrAdd(ReadOnlyMemoryValue<TData> data,
            ReadOnlyMemoryValue<TLink> link)
        {
            var neuron = wrap.Nerve.FindOrAddNeuron(data);
            var cacheKey = wrap.Nerve.CreateNeuronConnectionCacheKey(wrap.Cell, in neuron, in link.Value);
            return wrap.FindCore(in cacheKey, in neuron, in link.Value) ?? wrap.AddCore(cacheKey, neuron, link);
        }

        public async ValueTask<Connection> FindOrAddAsync(ReadOnlyMemoryValue<TData> data,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            var neuron = await wrap.Nerve.FindOrAddNeuronAsync(data, cancellationToken);
            var cacheKey = wrap.Nerve.CreateNeuronConnectionCacheKey(wrap.Cell, in neuron, in link.Value);
            return wrap.FindCore(in cacheKey, in neuron, in link.Value) ??
                   await wrap.AddAsyncCore(cacheKey, neuron, link, cancellationToken);
        }


        private Connection AddCore(NerveCacheKey cacheKey,
            Neuron neuron,
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
                        Link = link.Value
                    });

                neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;

                wrap.Nerve.TrySetNeuronConnectionCacheCore(in cacheKey, connectionValue.Offset);
                return new(connectionValue.Offset);
            });
        }


        private async ValueTask<Connection> AddAsyncCore(
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
            Neuron neuron,
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
                            Link = link.Value
                        });


                    neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;

                    wrap.Nerve.TrySetNeuronConnectionCacheCore(in cacheKey.Value, connectionValue.Offset);
                    return new(connectionValue.Offset);
                },
                cancellationToken);
        }

        public DataOffset? LastLoadedConnection
        {
            get => wrap.Nerve.LastLoadedConnectionCache.GetOrNull(wrap.Cell.Offset);
            set => wrap.Nerve.LastLoadedConnectionCache.Set(wrap.Cell.Offset, value ?? DataOffset.Null);
        }

        public IEnumerable<Connection> GetUnloadedConnections()
        {
            var unloaded = wrap.LastLoadedConnection;
            if (unloaded?.IsNull == true)
                yield break;

            var connection = unloaded.HasValue ? new Connection(unloaded.Value) : wrap.Connection;
            while (connection.HasValue)
            {
                var connectionWrap = connection.Value.Wrap(wrap);
                wrap.Nerve.TrySetNeuronConnectionCache(wrap.Cell, connectionWrap);
                wrap.Nerve.TrySetNeuronCache(connectionWrap.NeuronWrap);
                wrap.LastLoadedConnection = connectionWrap.Next?.Offset;
                connection = connectionWrap.Next;
                yield return connectionWrap.Cell;
            }
        }


        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetUnloadedConnectionsWrap() =>
            wrap.GetUnloadedConnections().Select(x => x.Wrap(wrap));
    }
}