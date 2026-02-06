using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NeuronWrapExtensions
{
    extension<TData, TLink>(CellWrap<Neuron, NeuronValue<TData>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public ref readonly TData RefData => ref wrap.RefValue.Data;
        public int Length => wrap.ConnectionWrap?.RefValue.NextCount + 1 ?? 0;

        public Connection? Connection =>
            wrap.RefValue.Connection.IsNull
                ? null
                : new(wrap.RefValue.Connection);

        public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>? ConnectionWrap =>
            wrap.Connection?.Wrap(wrap.Nerve);

        public CellEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap()
        {
            var childWrap = wrap.ConnectionWrap;
            return childWrap.HasValue
                ? new(
                    childWrap.Value.RefValue.NextCount + 1,
                    CellWrap<Neuron, NeuronValue<TData>, TData, TLink>.GetConnectionsWrapCore(childWrap.Value))
                : CellEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.Empty;
        }

        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapRaw() =>
            wrap.ConnectionWrap is { } childWrap
                ? CellWrap<Neuron, NeuronValue<TData>, TData, TLink>.GetConnectionsWrapCore(childWrap)
                : [];

        private static IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCore(
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> childWrap)
        {
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>? current = childWrap;
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

            var connection = unloaded.HasValue ? new Connection(unloaded.Value) : wrap.Connection;
            while (connection.HasValue)
            {
                var connectionWrap = connection.Value.Wrap(wrap);
                wrap.Nerve.TrySetNeuronConnectionCache(wrap.Cell, connectionWrap);
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
            if (wrap.Nerve.TryGetNeuronConnectionCacheCore(in cacheKey, out var connectionOffset))
                return new(connectionOffset.Value);

            var localNeuron = neuron;
            var localLink = link;
            var cellMemory = wrap.GetConnectionsWrap();
            var result = cellMemory
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

        public async ValueTask<Connection> FindOrAddAsync(ReadOnlyMemoryValue<TData> data,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            var neuron = await wrap.Nerve.FindOrAddNeuronAsync(data, cancellationToken);
            var cacheKey = wrap.Nerve.CreateNeuronConnectionCacheKey(wrap.Cell, in neuron, in link.Value);
            return wrap.FindCore(in cacheKey, in neuron, in link.Value) ??
                   await wrap.AddAsyncCore(cacheKey, neuron, link, cancellationToken);
        }

        private async ValueTask<Connection> AddAsyncCore(
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
            Neuron neuron,
            ReadOnlyMemoryValue<TLink> link,
            CancellationToken cancellationToken = default)
        {
            return await wrap.LockAsync(valueWrap =>
                {
                    if (wrap.FindCore(in cacheKey.Value, in neuron, in link.Value) is { } connection)
                        return connection;

                    var connectionValue = wrap.Nerve.Access
                        .Create(ConnectionValue<TLink>.Default with
                        {
                            Neuron = neuron.Offset,
                            Next = valueWrap.RefValue.Connection,
                            NextCount = wrap.ConnectionWrap?.RefValue.NextCount + 1 ?? 0,
                            Link = link.Value
                        });
                    Interlocked.Increment(ref wrap.Nerve.Counter.RefValue.ConnectionCount);


                    valueWrap.RefValue.Connection = connectionValue.Offset;

                    wrap.Nerve.TrySetNeuronConnectionCacheCore(in cacheKey.Value, connectionValue.Offset);
                    return new(connectionValue.Offset);
                },
                cancellationToken);
        }
    }
}