namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NeuronWrapExtensions
{
    extension<TData, TLink>(CellWrap<Neuron, NeuronValue<TData>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public ref readonly TData RefData
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return ref wrap.RefValue.Data;
            }
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return wrap.ConnectionWrap is { HasValue: true } connectionWrap
                    ? connectionWrap.Value.RefValue.NextCount + 1
                    : 0;
            }
        }

        public Optional<Connection> Connection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                ref readonly var refValueConnection = ref wrap.RefValue.Connection;
                return refValueConnection.IsNull
                    ? Optional<Connection>.Null
                    : Optional<Connection>.From(new(refValueConnection));
            }
        }

        public Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> ConnectionWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return wrap.Connection is { HasValue: true } connection
                    ? connection.Value.Wrap(wrap.Nerve)
                    : Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.Null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public CellEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrap()
        {
            var childWrap = wrap.ConnectionWrap;
            return childWrap.HasValue
                ? new(
                    childWrap.Value.RefValue.NextCount + 1,
                    CellWrap<Neuron, NeuronValue<TData>, TData, TLink>.GetConnectionsWrapCore(childWrap.Value))
                : CellEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapRaw()
        {
            return wrap.ConnectionWrap is { HasValue: true } childWrap
                ? CellWrap<Neuron, NeuronValue<TData>, TData, TLink>.GetConnectionsWrapCore(childWrap.Value)
                : [];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static IEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetConnectionsWrapCore(
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> childWrap)
        {
            Optional<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> current = childWrap;
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

            var connection = unloaded.HasValue ? new Connection(unloaded.Value) : wrap.Connection;
            while (connection.HasValue)
            {
                var connectionWrap = connection.Value.Wrap(wrap);
                wrap.Nerve.SetNeuronConnectionCache(wrap.RefCell, connectionWrap);
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
            if (wrap.Nerve.TryGetNeuronConnectionCacheCore(in cacheKey, out var connectionOffset))
            {
                return Optional<Connection>.From(new(connectionOffset.Value));
            }

            var localNeuron = neuron;
            var localLink = link;
            var cellMemory =
                wrap.GetConnectionsWrap();
            Optional<Connection> result = cellMemory
                .FirstOrDefault(x =>
                    x.RefValue.Neuron == localNeuron.Offset &&
                    x.RefLink.Equals(localLink) &&
                    x.RefValue.Previous.IsNull)
                .NullWhenDefault()
                ?.RefCell;

            if (result.HasValue)
            {
                wrap.Nerve.SetNeuronConnectionCacheCore(in cacheKey, result.Value.Offset);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Connection FindOrAdd(in TData data, in TLink link)
        {
            var neuron = wrap.Nerve.FindOrAddNeuron(in data);
            var cacheKey = wrap.Nerve.CreateNeuronConnectionCacheKey(wrap.RefCell, in neuron, in link);
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

            var location = wrap.Location;
            ref var locationRefValue = ref location.RefValue;
            wrap.Nerve.Access
                .Create(ConnectionValue<TLink>.Default with
                    {
                        Neuron = neuron.Offset,
                        Next = locationRefValue.Connection,
                        NextCount = wrap.ConnectionWrap is { HasValue: true } connectionWrap
                            ? connectionWrap.Value.RefValue.NextCount + 1
                            : 0,
                        Link = link
                    },
                    out var connectionValue);
            Interlocked.Increment(ref wrap.Nerve.Counter.RefValue.ConnectionCount);


            locationRefValue.Connection = connectionValue.Offset;

            wrap.Nerve.SetNeuronConnectionCacheCore(in cacheKey, in connectionValue.Offset);
            return new(connectionValue.Offset);
        }
    }
}