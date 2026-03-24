namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapExtensions
{
    extension<TData, TLink>(CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public ref readonly TLink RefLink
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return ref wrap.RefValue.Link;
            }
        }

        public Neuron Neuron
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return new(wrap.RefValue.Neuron);
            }
        }

        public CellWrap<Neuron, NeuronValue<TData>, TData, TLink> NeuronWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return wrap.Neuron.Wrap(wrap.Nerve);
            }
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                var wrapChildWrap = wrap.ChildWrap;
                return wrapChildWrap.HasValue ? wrapChildWrap.Value.RefValue.NextCount + 1 : 0;
            }
        }

        public Optional<Connection> Previous
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                ref readonly var refValuePrevious = ref wrap.RefValue.Previous;
                return refValuePrevious.IsNull
                    ? Optional<Connection>.Null
                    : Optional<Connection>.From(new(in refValuePrevious));
            }
        }

        public Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> PreviousWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                ref readonly var refValuePrevious = ref wrap.RefValue.Previous;
                return refValuePrevious.IsNull
                    ? Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.Null
                    : Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.From(new(wrap.Nerve,
                        new(refValuePrevious)));
            }
        }

        public Optional<Connection> Next
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                ref readonly var refValueNext = ref wrap.RefValue.Next;
                return refValueNext.IsNull
                    ? Optional<Connection>.Null
                    : Optional<Connection>.From(new(refValueNext));
            }
        }

        public Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> NextWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                ref readonly var refValueNext = ref wrap.RefValue.Next;
                return refValueNext.IsNull
                    ? Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.Null
                    : Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.From(new(wrap.Nerve,
                        new(refValueNext)));
            }
        }

        public Optional<Connection> Child
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                ref readonly var refValueChild = ref wrap.RefValue.Child;
                return refValueChild.IsNull
                    ? Optional<Connection>.Null
                    : Optional<Connection>.From(new(refValueChild));
            }
        }

        public Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> ChildWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                ref readonly var refValueChild = ref wrap.RefValue.Child;
                return refValueChild.IsNull
                    ? Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.Null
                    : Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.From(new(wrap.Nerve,
                        new(refValueChild)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public CellEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap()
        {
            var childWrap = wrap.ChildWrap;
            return childWrap.HasValue
                ? new(
                    childWrap.Value.RefValue.NextCount + 1,
                    CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>.GetConnectionsWrapCore(childWrap.Value))
                : CellEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public CellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCache()
        {
            return wrap.Nerve.MemoryCache.GetOrAdd(wrap.RefCell.Offset,
                    static (_, wrap) => new(() =>
                            wrap.GetConnectionsWrap().ToCellMemory(),
                        LazyThreadSafetyMode.ExecutionAndPublication),
                    wrap)
                .Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapRaw()
        {
            return wrap.ChildWrap is { HasValue: true } childWrap
                ? CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>.GetConnectionsWrapCore(childWrap.Value)
                : [];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCore(
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> cellWrap)
        {
            Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> current = cellWrap;
            while (current.HasValue)
            {
                yield return current.Value;
                current = current.Value.NextWrap;
            }
        }

        public DataOffset? LastLoadedConnection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return wrap.Nerve.LastLoadedConnectionCache.GetOrNull(wrap.RefCell.Offset);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            set
            {
                wrap.Nerve.LastLoadedConnectionCache.Set(wrap.RefCell.Offset, value ?? DataOffset.Null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetUnloadedConnectionsWrap()
        {
            var unloaded = wrap.LastLoadedConnection;
            if (unloaded?.IsNull == true)
            {
                yield break;
            }

            var connection = unloaded.HasValue ? new Connection(unloaded.Value) : wrap.Child;
            while (connection.HasValue)
            {
                var connectionWrap = connection.Value.Wrap(wrap);
                wrap.Nerve.SetConnectionCache(connectionWrap);
                wrap.Nerve.SetNeuronCache(connectionWrap.NeuronWrap);
                wrap.LastLoadedConnection = connectionWrap.RefValue.Next;
                connection = connectionWrap.Next;
                yield return connectionWrap;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private Optional<Connection> FindCore(in NerveCacheKey cacheKey,
            in Neuron neuron,
            in TLink link)
        {
            if (wrap.Nerve.TryGetConnectionCacheCore(in cacheKey, out var connectionOffset))
            {
                return Optional<Connection>.From(new(connectionOffset.Value));
            }

            var localNeuron = neuron;
            var localLink = link;
            var cellMemory =
                wrap.GetUnloadedConnectionsWrap();
            Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> result = cellMemory
                .FirstOrDefault(x =>
                    x.RefValue.Neuron == localNeuron.Offset && x.RefLink.Equals(in localLink))
                .NullWhenDefault();

            if (result.HasValue)
            {
                wrap.Nerve.SetConnectionCacheCore(in cacheKey, in result.Value.RefCell.Offset);
            }

            return result.HasValue ? result.Value.RefCell : Optional<Connection>.Null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Connection FindOrAdd(in Neuron neuron, in TLink link)
        {
            var cacheKey = wrap.Nerve.CreateConnectionCacheKey(in wrap, in neuron, in link);
            return wrap.FindCore(in cacheKey, in neuron, in link) is { HasValue: true } optional
                ? optional.Value
                : wrap.AddCore(in cacheKey, in neuron, in link);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private Connection AddCore(
            in NerveCacheKey cacheKey,
            in Neuron neuron,
            in TLink link)
        {
            using var @lock = wrap.Lock();
            if (wrap.FindCore(in cacheKey, in neuron, in link) is { HasValue: true } connection)
            {
                return connection.Value;
            }

            wrap.Nerve.Access
                .Create(ConnectionValue<TLink>.Default with
                {
                    Previous = wrap.RefCell.Offset,
                    Neuron = neuron.Offset,
                    Link = link,
                    Next = wrap.RefValue.Child,
                    NextCount = wrap.ChildWrap is { HasValue: true } childWrap
                            ? childWrap.Value.RefValue.NextCount + 1
                            : 0
                },
                    out var connectionValue);
            Interlocked.Increment(ref wrap.Nerve.Counter.RefValue.ConnectionCount);

            wrap.Location.RefValue.Child = connectionValue.Offset;

            wrap.Nerve.SetConnectionCacheCore(in cacheKey, connectionValue.Offset);
            return new(connectionValue.Offset);
        }
    }
}