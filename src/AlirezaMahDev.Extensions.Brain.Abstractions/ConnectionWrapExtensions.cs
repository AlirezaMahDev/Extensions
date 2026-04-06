using System.Diagnostics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapExtensions
{
    extension<TData, TLink>(in CellWrap<ConnectionValue<TLink>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public CellWrap<NeuronValue<TData>, TData, TLink> NeuronWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Location.ReadLock((scoped ref readonly x, scoped ref readonly nerve) =>
                    x.Neuron.NewWrap(nerve),
                wrap.Nerve);
        }

        public Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>> NextWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Location.ReadLock((scoped ref readonly x, scoped ref readonly nerve) =>
                    x.Next.Offset.IsNull
                        ? Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>>.Null
                        : x.Next.NewWrap(nerve),
                wrap.Nerve);
        }

        public Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>> ChildWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Location.ReadLock((scoped ref readonly x, scoped ref readonly nerve) =>
                    x.Child.Offset.IsNull
                        ? Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>>.Null
                        : x.Child.NewWrap(nerve),
                wrap.Nerve);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public CellEnumerable<CellWrap<ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap()
        {
            using var @lock = wrap.Location.ReadLock();
            var count = @lock.RefReadOnlyValue.Count;
            return count > 0 && wrap.ChildWrap is { HasValue: true } childWrap
                ? new(count,
                    CellWrap<ConnectionValue<TLink>, TData, TLink>.GetConnectionsWrapCore(childWrap.Value))
                : CellEnumerable<CellWrap<ConnectionValue<TLink>, TData, TLink>>.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public CellMemory<CellWrap<ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCache()
        {
            return wrap.Nerve.MemoryCache.GetOrAdd(wrap.Location.Offset,
                    static (_, wrap) => new(() =>
                            wrap.GetConnectionsWrap().ToCellMemory(),
                        LazyThreadSafetyMode.ExecutionAndPublication),
                    wrap)
                .Value;
        }

        public DataOffset? NextUnloadItem
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Nerve.NextUnloadItem.GetOrNull(in wrap.Location.Offset);
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            set
            {
                var offset = value ?? DataOffset.Null;
                wrap.Nerve.NextUnloadItem.Set(in wrap.Location.Offset, in offset);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private Optional<Connection> FindCore(ref readonly NerveCacheKey cacheKey,
            ref readonly TLink link,
            ref readonly Neuron neuron)
        {
            if (wrap.Nerve.TryGetConnectionCacheCore(in cacheKey, out var connection))
            {
                if (connection.Offset.IsDefault)
                    Debugger.Break();
                return Optional<Connection>.From(connection);
            }

            var localNeuron = neuron;
            var localLink = link;

            var cellMemory = wrap.GetUnloadedConnectionsWrap();
            var result = cellMemory
                .FirstOrDefault(x =>
                    x.Location.ReadLock((scoped ref readonly connectionValue, scoped ref readonly innerLocalLink) =>
                            connectionValue.Neuron == localNeuron && connectionValue.Link == innerLocalLink,
                        in localLink))
                .NullWhenDefault();

            if (result?.Location.Offset.IsDefault == true)
                Debugger.Break();

            return result.HasValue
                ? Optional<Connection>.From(new(result.Value.Location.Offset))
                : Optional<Connection>.Null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Connection FindOrAdd(ref readonly TLink link, ref readonly Neuron neuron)
        {
            var parent = new Connection(wrap.Location.Offset);
            var cacheKey = INerve<TData, TLink>.CreateConnectionCacheKey(in parent, in link, in neuron);
            return wrap.FindCore(in cacheKey, in link, in neuron) is { HasValue: true } optional
                ? optional.Value
                : wrap.AddCore(in cacheKey, in link, in neuron);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private Connection AddCore(ref readonly NerveCacheKey cacheKey,
            ref readonly TLink link,
            ref readonly Neuron neuron)
        {
            using var parent = wrap.Location.WriteLock();
            if (wrap.FindCore(in cacheKey, in link, in neuron) is { HasValue: true } connection)
            {
                if (connection.Value.Offset.IsDefault)
                    Debugger.Break();
                return connection.Value;
            }

            wrap.Nerve.Access
                .Create(ConnectionValue<TLink>.Default with
                    {
                        Link = link,
                        Neuron = neuron,
                        Next = parent.RefValue.Child
                    },
                    out var connectionValue);

            Connection newConnection = new(connectionValue.Offset);
            parent.RefValue.Child = newConnection;
            parent.RefValue.Count++;
            wrap.Nerve.GetOrAddConnectionCacheCore(in cacheKey, in newConnection);

            if (newConnection.Offset.IsDefault)
                Debugger.Break();

            return newConnection;
        }
    }

    extension<TData, TLink>(CellWrap<ConnectionValue<TLink>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static IEnumerable<CellWrap<ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCore(
            CellWrap<ConnectionValue<TLink>, TData, TLink> cellWrap)
        {
            Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>> current = cellWrap;
            while (current.HasValue)
            {
                yield return current.Value;
                current = current.Value.NextWrap;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<CellWrap<ConnectionValue<TLink>, TData, TLink>> GetUnloadedConnectionsWrap()
        {
            var unloaded = wrap.NextUnloadItem;
            if (unloaded?.IsNull == true)
            {
                yield break;
            }

            var lastConnectionWrap = unloaded.HasValue
                ? new Connection(unloaded.Value).NewWrap(wrap.Nerve)
                : wrap.ChildWrap;
            while (lastConnectionWrap.HasValue)
            {
                var connectionWrap = lastConnectionWrap.Value;
                connectionWrap.Location.ReadLock((scoped ref readonly value) =>
                {
                    var parent = new Connection(wrap.Location.Offset);
                    var current = new Connection(connectionWrap.Location.Offset);
                    wrap.Nerve.GetOrAddConnectionCache(in parent,
                        in value.Link,
                        in value.Neuron,
                        in current);
                    wrap.NextUnloadItem = value.Next.Offset;
                });
                wrap.NextUnloadItem = connectionWrap.Location.ReadLock((scoped ref readonly x) => x.Next.Offset);
                lastConnectionWrap = connectionWrap.NextWrap;
                yield return connectionWrap;
            }
        }
    }
}