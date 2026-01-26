using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

using JetBrains.Annotations;

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

        [MustDisposeResource]
        public CellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap() =>
            CellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.Create(
                wrap.RefValue.ChildCount,
                wrap.GetConnectionsWrapCore());

        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCore()
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
            using var cellMemory = wrap.GetConnectionsWrap();
            var result = cellMemory
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
            return wrap.Lock(valueWrap =>
            {
                if (wrap.FindCore(in cacheKey, in neuron, in link.Value) is { } connection)
                    return connection;

                var connectionValue = wrap.Nerve.Access
                    .Create(ConnectionValue<TLink>.Default with
                    {
                        Previous = wrap.Cell.Offset,
                        Neuron = neuron.Offset,
                        Link = link.Value,
                        Next = valueWrap.RefValue.Child
                    });

                valueWrap.RefValue.Child = connectionValue.Offset;
                valueWrap.RefValue.ChildCount++;

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
                            Next = wrap.RefValue.Child
                        });

                    valueWrap.RefValue.Child = connectionValue.Offset;
                    valueWrap.RefValue.ChildCount++;
                    
                    wrap.Nerve.TrySetConnectionCacheCore(in cacheKey, connectionValue.Offset);
                    return new(connectionValue.Offset);
                },
                cancellationToken);
        }
    }
}