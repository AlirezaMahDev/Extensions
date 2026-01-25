using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionCellWrapExtensions
{
    extension<TData, TLink>(CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public ref readonly TLink RefLink => ref wrap.Location.RefValue.Link;

        public Neuron Neuron => new(wrap.RefValue.Neuron);
        public CellWrap<Neuron, NeuronValue<TData>, TData, TLink> NeuronWrap => wrap.Neuron.Wrap(wrap.Nerve);

        public Connection? Previous =>
            wrap.RefValue.Previous.IsNull
                ? null
                : new(wrap.RefValue.Previous);

        public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>? PreviousWrap =>
            wrap.RefValue.Previous.IsNull
                ? null
                : new(wrap.Nerve, new(wrap.RefValue.Previous));

        public Connection? Next =>
            wrap.RefValue.Next.IsNull
                ? null
                : new(wrap.RefValue.Next);

        public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>? NextWrap =>
            wrap.RefValue.Next.IsNull
                ? null
                : new(wrap.Nerve, new(wrap.RefValue.Next));

        public Connection? Child =>
            wrap.RefValue.Child.IsNull
                ? null
                : new(wrap.RefValue.Child);

        public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>? ChildWrap =>
            wrap.RefValue.Child.IsNull
                ? null
                : new(wrap.Nerve, new(wrap.RefValue.Child));

        public IEnumerable<Connection> GetConnections()
        {
            var current = wrap.Child;
            while (current.HasValue)
            {
                yield return current.Value;
                current = current.Value.Wrap(wrap.Nerve).Next;
            }
        }

        public DataOffset? LastLoadedConnection
        {
            get => wrap.Nerve.LastLoadedConnectionCache.GetOrNull(wrap.Cell.Offset);
            set => wrap.Nerve.LastLoadedConnectionCache.Set(wrap.Cell.Offset, value ?? DataOffset.Null);
        }

        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap()
        {
            var current = wrap.ChildWrap;
            while (current.HasValue)
            {
                yield return current.Value;
                current = current.Value.NextWrap;
            }
        }

        public Connection? Find(in Neuron neuron, in TLink link)
        {
            var cacheKey = wrap.Nerve.CreateConnectionCacheKey(in wrap, in neuron, in link);
            return wrap.FindCore(in cacheKey, in neuron, in link);
        }

        private Connection? FindCore(in NerveCacheKey cacheKey,
            in Neuron neuron,
            in TLink link)
        {
            if (wrap.Nerve.TryGetConnectionCacheCore(in cacheKey, out var connectionOffset))
                return new(connectionOffset.Value);

            var localNeuron = neuron;
            var localLink = link;
            var result = wrap.GetUnloadedConnectionsWrap()
                .FirstOrDefault(x =>
                    x.RefValue.Neuron == localNeuron.Offset && x.RefLink.Equals(localLink))
                .NullWhenDefault();

            if (result.HasValue)
                wrap.Nerve.TrySetConnectionCacheCore(in cacheKey, result.Value.Cell.Offset);

            return result?.Cell;
        }

        public Connection FindOrAdd(in Neuron neuron, in TLink link)
        {
            var cacheKey = wrap.Nerve.CreateConnectionCacheKey(in wrap, in neuron, in link);
            return wrap.FindCore(in cacheKey, in neuron, in link) ?? wrap.AddCore(cacheKey, neuron, link);
        }

        public async ValueTask<Connection> FindOrAddAsync(Neuron neuron,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = wrap.Nerve.CreateConnectionCacheKey(in wrap, in neuron, in link.Value);
            return wrap.FindCore(in cacheKey, in neuron, in link.Value) ??
                   await wrap.AddAsyncCore(cacheKey, neuron, link, cancellationToken);
        }

        private Connection AddCore(NerveCacheKey cacheKey,
            Neuron neuron,
            ReadOnlyMemoryValue<TLink> link)
        {
            return wrap.Lock(connectionDataLocation =>
            {
                if (wrap.FindCore(in cacheKey, in neuron, in link.Value) is { } connection)
                    return connection;

                var connectionValue = wrap.Nerve.Access
                    .Create(ConnectionValue<TLink>.Default with
                    {
                        Previous = wrap.Cell.Offset,
                        Neuron = neuron.Offset,
                        Link = link.Value,
                        Next = wrap.RefValue.Child
                    });

                connectionDataLocation.Wrap(wrap.Nerve.Access).RefValue.Child = connectionValue.Offset;

                wrap.Nerve.TrySetConnectionCacheCore(in cacheKey, connectionValue.Offset);
                return new(connectionValue.Offset);
            });
        }

        private async ValueTask<Connection> AddAsyncCore(
            NerveCacheKey cacheKey,
            Neuron neuron,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            return await wrap.LockAsync(connectionDataLocation =>
                {
                    if (wrap.FindCore(in cacheKey, in neuron, in link.Value) is { } connection)
                        return connection;

                    var connectionValue = wrap.Nerve.Access
                        .Create(ConnectionValue<TLink>.Default with
                        {
                            Previous = wrap.Cell.Offset,
                            Neuron = neuron.Offset,
                            Link = link.Value,
                            Next = wrap.RefValue.Child
                        });

                    connectionDataLocation.Wrap(wrap.Nerve.Access).RefValue.Child = connectionValue.Offset;

                    wrap.Nerve.TrySetConnectionCacheCore(in cacheKey, connectionValue.Offset);
                    return new(connectionValue.Offset);
                },
                cancellationToken);
        }

        public IEnumerable<Connection> GetUnloadedConnections()
        {
            var unloaded = wrap.LastLoadedConnection;
            if (unloaded?.IsNull == true)
                yield break;

            var connection = unloaded.HasValue ? new Connection(unloaded.Value) : wrap.Child;
            while (connection.HasValue)
            {
                var connectionWrap = connection.Value.Wrap(wrap);
                wrap.Nerve.TrySetConnectionCache(connectionWrap);
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