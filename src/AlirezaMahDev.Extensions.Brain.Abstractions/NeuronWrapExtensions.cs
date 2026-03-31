namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NeuronWrapExtensions
{
    extension<TData, TLink>(in CellWrap<NeuronValue<TData>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>> ConnectionWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Location.ReadLock((scoped ref readonly x, scoped in nerve) =>
                x.Connection.Offset.IsNull
                    ? Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>>.Null
                    : x.Connection.NewWrap(nerve),
                    wrap.Nerve);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public CellEnumerable<CellWrap<ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap()
        {
            var childWrap = wrap.ConnectionWrap;
            return childWrap.HasValue
                ? new(
                    childWrap.Value.Location.ReadLock((scoped ref readonly x) => x.NextCount) + 1,
                    CellWrap<NeuronValue<TData>, TData, TLink>.GetConnectionsWrapCore(childWrap.Value))
                : CellEnumerable<CellWrap<ConnectionValue<TLink>, TData, TLink>>.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private Optional<Connection> FindCore(scoped ref readonly NerveCacheKey cacheKey,
            scoped ref readonly Neuron neuron,
            scoped ref readonly TLink link)
        {
            if (wrap.Nerve.TryGetNeuronConnectionCacheCore(in cacheKey, out var connectionOffset))
            {
                return Optional<Connection>.From(new(connectionOffset.Value));
            }

            var localNeuron = neuron;
            var localLink = link;
            var cellMemory =
                wrap.GetConnectionsWrap();
            Optional<DataOffset> result = cellMemory
                .FirstOrDefault(x =>
                {
                    using var connectionValue = x.Location.ReadLock();
                    return connectionValue.RefReadOnlyValue.Neuron == localNeuron &&
                           connectionValue.RefReadOnlyValue.Link == localLink &&
                           connectionValue.RefReadOnlyValue.Previous.Offset.IsNull;
                })
                .NullWhenDefault()
                ?.Location.Offset;

            if (result.HasValue)
            {
                wrap.Nerve.TrySetNeuronConnectionCacheCore(in cacheKey, in result.Value);
                return Optional<Connection>.From(new(result.Value));
            }

            return Optional<Connection>.Null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Connection FindOrAdd(scoped ref readonly TData data, scoped ref readonly TLink link)
        {
            var neuron = wrap.Nerve.FindOrAddNeuron(in data);
            var cacheKey =
                wrap.Nerve.CreateNeuronConnectionCacheKey(in wrap.Location.Offset, in neuron.Offset, in link);
            return wrap.FindCore(ref cacheKey, ref neuron, in link) is { HasValue: true } optional
                ? optional.Value
                : wrap.AddCore(ref cacheKey, ref neuron, in link);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private Connection AddCore(
            scoped ref readonly NerveCacheKey cacheKey,
            scoped ref readonly Neuron neuron,
            scoped ref readonly TLink link)
        {
            using var neuronValue = wrap.LocationWrap.Location.WriteLock();
            if (wrap.FindCore(in cacheKey, in neuron, in link) is { HasValue: true } connection)
            {
                return connection.Value;
            }

            ref var locationRefValue = ref neuronValue.RefValue;
            wrap.Nerve.Access
                .Create(ConnectionValue<TLink>.Default with
                {
                    Neuron = neuron,
                    Next = locationRefValue.Connection,
                    NextCount = wrap.ConnectionWrap is { HasValue: true } connectionWrap
                            ? connectionWrap.Value.Location.ReadLock((scoped ref readonly x) => x.NextCount) + 1
                            : 0,
                    Link = link
                },
                    out var connectionValue);
            Interlocked.Increment(ref wrap.Nerve.Counter.UnsafeRefValue.ConnectionCount);


            locationRefValue.Connection = new(connectionValue.Offset);

            wrap.Nerve.TrySetNeuronConnectionCacheCore(in cacheKey, in connectionValue.Offset);
            return new(connectionValue.Offset);
        }
        public DataOffset? LastLoadedConnection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Nerve.LastLoadedConnectionCache.GetOrNull(in wrap.Location.Offset);
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            set
            {
                var dataOffset = value ?? DataOffset.Null;
                wrap.Nerve.LastLoadedConnectionCache.Set(in wrap.Location.Offset, in dataOffset);
            }
        }
    }

    extension<TData, TLink>(CellWrap<NeuronValue<TData>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<CellWrap<ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapRaw()
        {
            return wrap.ConnectionWrap is { HasValue: true } childWrap
                ? CellWrap<NeuronValue<TData>, TData, TLink>.GetConnectionsWrapCore(childWrap.Value)
                : [];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static IEnumerable<CellWrap<ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCore(
            CellWrap<ConnectionValue<TLink>, TData, TLink> childWrap)
        {
            Optional<CellWrap<ConnectionValue<TLink>, TData, TLink>> current = childWrap;
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

            var connection = unloaded.HasValue ? new Connection(unloaded.Value).NewWrap(wrap.Nerve) : wrap.ConnectionWrap;
            while (connection.HasValue)
            {
                var connectionWrap = connection.Value;
                wrap.Nerve.TrySetNeuronConnectionCache(in wrap.Location.Offset, in connectionWrap);
                var connectionWrapNeuronWrap = connectionWrap.NeuronWrap;
                wrap.Nerve.TrySetNeuronCache(in connectionWrapNeuronWrap);
                wrap.LastLoadedConnection = connectionWrap.Location.ReadLock((scoped ref readonly x) => x.Next.Offset);
                connection = connectionWrap.NextWrap;
                yield return connectionWrap;
            }
        }
    }
}