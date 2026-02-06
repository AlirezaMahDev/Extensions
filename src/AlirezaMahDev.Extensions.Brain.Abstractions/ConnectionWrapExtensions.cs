using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.Caching.Memory;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapExtensions
{
    extension<TData, TLink>(CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public ref readonly TLink RefLink => ref wrap.RefValue.Link;

        public Neuron Neuron => new(wrap.RefValue.Neuron);
        public CellWrap<Neuron, NeuronValue<TData>, TData, TLink> NeuronWrap => wrap.Neuron.Wrap(wrap.Nerve);
        public int Length => wrap.ChildWrap?.RefValue.NextCount + 1 ?? 0;

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

        public CellEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap()
        {
            var childWrap = wrap.ChildWrap;
            return childWrap.HasValue
                ? new(
                    childWrap.Value.RefValue.NextCount + 1,
                    CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>.GetConnectionsWrapCore(childWrap.Value))
                : CellEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.Empty;
        }

        public ICellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCache() =>
            wrap.Nerve.MemoryCache.GetOrCreate(wrap.Cell.Offset,
                entry =>
                {
                    var result = wrap.GetConnectionsWrap().ToCellMemory();
                    entry.PostEvictionCallbacks.Add(new() { EvictionCallback = (_, _, _, _) => result.Dispose() });
                    entry.Priority = CacheItemPriority.NeverRemove;
                    return result;
                })!;

        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapRaw() =>
            wrap.ChildWrap is { } childWrap
                ? CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>.GetConnectionsWrapCore(childWrap)
                : [];

        private static IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCore(
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> cellWrap)
        {
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>? current = cellWrap;
            while (current.HasValue)
            {
                yield return current.Value;
                current = current.Value.NextWrap;
            }
        }

        public DataOffset? LastLoadedConnection
        {
            get => wrap.Nerve.LastLoadedConnectionCache.GetOrNull(wrap.Cell.Offset);
            set => wrap.Nerve.LastLoadedConnectionCache.Set(wrap.Cell.Offset, value ?? DataOffset.Null);
        }

        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetUnloadedConnectionsWrap()
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
                wrap.LastLoadedConnection = connectionWrap.RefValue.Next;
                connection = connectionWrap.Next;
                yield return connectionWrap;
            }
        }

        private Connection? FindCore(in NerveCacheKey cacheKey,
            in Neuron neuron,
            in TLink link)
        {
            if (wrap.Nerve.TryGetConnectionCacheCore(in cacheKey, out var connectionOffset))
                return new(connectionOffset.Value);

            var localNeuron = neuron;
            var localLink = link;
            var cellMemory = wrap.GetUnloadedConnectionsWrap();
            var result = cellMemory
                .FirstOrDefault(x =>
                    x.RefValue.Neuron == localNeuron.Offset && x.RefLink.Equals(localLink))
                .NullWhenDefault();

            if (result.HasValue)
                wrap.Nerve.TrySetConnectionCacheCore(in cacheKey, result.Value.Cell.Offset);

            return result?.Cell;
        }

        public async ValueTask<Connection> FindOrAddAsync(Neuron neuron,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = wrap.Nerve.CreateConnectionCacheKey(in wrap, in neuron, in link.Value);
            return wrap.FindCore(in cacheKey, in neuron, in link.Value) ??
                   await wrap.AddAsyncCore(cacheKey, neuron, link, cancellationToken);
        }

        private async ValueTask<Connection> AddAsyncCore(
            NerveCacheKey cacheKey,
            Neuron neuron,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            return await wrap.LockAsync(valueWrap =>
                {
                    if (wrap.FindCore(in cacheKey, in neuron, in link.Value) is { } connection)
                        return connection;

                    var connectionValue = wrap.Nerve.Access
                        .Create(ConnectionValue<TLink>.Default with
                        {
                            Previous = wrap.Cell.Offset,
                            Neuron = neuron.Offset,
                            Link = link.Value,
                            Next = wrap.RefValue.Child,
                            NextCount = wrap.ChildWrap?.RefValue.NextCount + 1 ?? 0
                        });
                    Interlocked.Increment(ref wrap.Nerve.Counter.RefValue.ConnectionCount);

                    valueWrap.RefValue.Child = connectionValue.Offset;

                    wrap.Nerve.TrySetConnectionCacheCore(in cacheKey, connectionValue.Offset);
                    return new(connectionValue.Offset);
                },
                cancellationToken);
        }
    }
}