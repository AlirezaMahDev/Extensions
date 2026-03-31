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
            get => wrap.Location.ReadLock((scoped ref readonly x, scoped in nerve) =>
                x.Neuron.NewWrap(nerve), wrap.Nerve);
        }

        public CellWrap<ConnectionValue<TLink>, TData, TLink> PreviousWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Location.ReadLock((scoped ref readonly x, scoped in nerve) =>
                x.Previous.NewWrap(nerve), wrap.Nerve);
        }

        public Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>> NextWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Location.ReadLock((scoped ref readonly x, scoped in nerve) =>
                x.Next.Offset.IsNull
                    ? Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>>.Null
                    : x.Next.NewWrap(nerve), wrap.Nerve);
        }

        public Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>> ChildWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Location.ReadLock((scoped ref readonly x, scoped in nerve) =>
                x.Child.Offset.IsNull
                    ? Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>>.Null
                    : x.Child.NewWrap(nerve), wrap.Nerve);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public CellEnumerable<CellWrap<ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap()
        {
            var childWrap = wrap.ChildWrap;
            return childWrap.HasValue
                ? new(
                    childWrap.Value.Location.ReadLock((scoped ref readonly x) => x.NextCount) + 1,
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

        public DataOffset? LastLoadedConnection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Nerve.LastLoadedConnectionCache.GetOrNull(in wrap.Location.Offset);
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            set
            {
                var offset = value ?? DataOffset.Null;
                wrap.Nerve.LastLoadedConnectionCache.Set(in wrap.Location.Offset, in offset);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private Optional<Connection> FindCore(ref readonly NerveCacheKey cacheKey,
            ref readonly Neuron neuron,
            ref readonly TLink link)
        {
            if (wrap.Nerve.TryGetConnectionCacheCore(in cacheKey, out var connectionOffset))
            {
                return Optional<Connection>.From(new(connectionOffset.Value));
            }

            var localNeuron = neuron;
            var localLink = link;
            var cellMemory =
                wrap.GetUnloadedConnectionsWrap();
            Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>> result = cellMemory
                .FirstOrDefault(x =>
                    x.Location.ReadLock((scoped ref readonly connectionValue, scoped in innerLocalLink) =>
                        connectionValue.Neuron == localNeuron && connectionValue.Link == innerLocalLink, in localLink))
                .NullWhenDefault();

            if (result.HasValue)
            {
                wrap.Nerve.TrySetConnectionCacheCore(in cacheKey, in result.Value.Location.Offset);
            }

            return result.HasValue
                ? Optional<Connection>.From(new(result.Value.Location.Offset))
                : Optional<Connection>.Null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Connection FindOrAdd(ref readonly Neuron neuron, ref readonly TLink link)
        {
            var cacheKey = wrap.Nerve.CreateConnectionCacheKey(in wrap, in neuron, in link);
            return wrap.FindCore(ref cacheKey, in neuron, in link) is { HasValue: true } optional
                ? optional.Value
                : wrap.AddCore(ref cacheKey, in neuron, in link);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private Connection AddCore(
            ref readonly NerveCacheKey cacheKey,
            ref readonly Neuron neuron,
            ref readonly TLink link)
        {
            using var parent = wrap.Location.WriteLock();
            if (wrap.FindCore(in cacheKey, in neuron, in link) is { HasValue: true } connection)
            {
                return connection.Value;
            }

            wrap.Nerve.Access
                .Create(ConnectionValue<TLink>.Default with
                {
                    Previous = new(wrap.Location.Offset),
                    Neuron = neuron,
                    Link = link,
                    Next = parent.RefValue.Child,
                    NextCount = wrap.ChildWrap is { HasValue: true } childWrap
                            ? childWrap.Value.Location.ReadLock((scoped ref readonly x) => x.NextCount) + 1
                            : 0
                },
                    out var connectionValue);
            Interlocked.Increment(ref wrap.Nerve.Counter.UnsafeRefValue.ConnectionCount);

            parent.RefValue.Child = new(connectionValue.Offset);

            wrap.Nerve.TrySetConnectionCacheCore(in cacheKey, in connectionValue.Offset);
            return new(connectionValue.Offset);
        }
    }

    extension<TData, TLink>(CellWrap<ConnectionValue<TLink>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<CellWrap<ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapRaw()
        {
            return wrap.ChildWrap is { HasValue: true } childWrap
                ? CellWrap<ConnectionValue<TLink>, TData, TLink>.GetConnectionsWrapCore(childWrap.Value)
                : [];
        }

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
            var unloaded = wrap.LastLoadedConnection;
            if (unloaded?.IsNull == true)
            {
                yield break;
            }

            var connection = unloaded.HasValue ? new Connection(unloaded.Value).NewWrap(wrap.Nerve) : wrap.ChildWrap;
            while (connection.HasValue)
            {
                var connectionWrap = connection.Value;
                wrap.Nerve.TrySetConnectionCache(in connectionWrap);
                var connectionWrapNeuronWrap = connectionWrap.NeuronWrap;
                wrap.Nerve.TrySetNeuronCache(in connectionWrapNeuronWrap);
                wrap.LastLoadedConnection = connectionWrap.Location.ReadLock((scoped ref readonly x) => x.Next.Offset);
                connection = connectionWrap.NextWrap;
                yield return connectionWrap;
            }
        }
    }
}