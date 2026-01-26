using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NeuronWrapExtensions
{
    extension<TData, TLink>(CellWrap<Neuron, NeuronValue<TData>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public ref readonly TData RefData => ref wrap.RefValue.Data;

        public Connection? Connection =>
            wrap.RefValue.Connection.IsNull
                ? null
                : new(wrap.RefValue.Connection);

        public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>? ConnectionWrap =>
            wrap.Connection?.Wrap(wrap.Nerve);


        [MustDisposeResource]
        public CellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap() =>
            CellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.Create(
                wrap.RefValue.ConnectionCount,
                wrap.GetConnectionsWrapCore());

        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCore()
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
            using var cellMemory = wrap.GetConnectionsWrap();
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
            return wrap.Lock(valueWrap =>
            {
                if (wrap.FindCore(in cacheKey, in neuron, in link.Value) is { } connection)
                {
                    return connection;
                }

                var connectionValue = wrap.Nerve.Access
                    .Create(ConnectionValue<TLink>.Default with
                    {
                        Neuron = neuron.Offset,
                        Next = valueWrap.RefValue.Connection,
                        Link = link.Value
                    });

                valueWrap.RefValue.Connection = connectionValue.Offset;
                valueWrap.RefValue.ConnectionCount++;

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
            return await wrap.LockAsync(valueWrap =>
                {
                    if (wrap.FindCore(in cacheKey.Value, in neuron, in link.Value) is { } connection)
                        return connection;

                    var connectionValue = wrap.Nerve.Access
                        .Create(ConnectionValue<TLink>.Default with
                        {
                            Neuron = neuron.Offset,
                            Next = valueWrap.RefValue.Connection,
                            Link = link.Value
                        });


                    valueWrap.RefValue.Connection = connectionValue.Offset;
                    valueWrap.RefValue.ConnectionCount++;

                    wrap.Nerve.TrySetNeuronConnectionCacheCore(in cacheKey.Value, connectionValue.Offset);
                    return new(connectionValue.Offset);
                },
                cancellationToken);
        }
    }
}