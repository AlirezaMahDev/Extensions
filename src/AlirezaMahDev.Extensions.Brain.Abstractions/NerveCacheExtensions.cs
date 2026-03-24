namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveCacheExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public INerveCacheSection NeuronSectionCache => nerve.Cache.GetOrAdd("n");
        public INerveCacheSection ConnectionSectionCache => nerve.Cache.GetOrAdd("c");
        public INerveCacheSection LastLoadedConnectionCache => nerve.Cache.GetOrAdd("l");
    }

    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public bool TryGetNeuronCache(in TData data, [NotNullWhen(true)] out Neuron? neuron)
        {
            if (nerve.TryGetNeuronCacheCore(nerve.CreateNeuronCacheKey(in data), out var offset))
            {
                neuron = new(offset.Value);
                return true;
            }

            neuron = null;
            return false;
        }

        public bool TryGetNeuronCacheCore(in NerveCacheKey cacheKey, [NotNullWhen(true)] out DataOffset? offset)
        {
            return nerve.NeuronSectionCache.TryGet(in cacheKey, out offset);
        }

        public void SetNeuronCache(in CellWrap<Neuron, NeuronValue<TData>, TData, TLink> neuronWrap)
        {
            nerve.SetNeuronCacheCore(
                nerve.CreateNeuronCacheKey(in neuronWrap),
                neuronWrap.RefCell.Offset);
        }

        public void SetNeuronCacheCore(in NerveCacheKey cacheKey, in DataOffset offset)
        {
            nerve.NeuronSectionCache.Set(in cacheKey, offset);
        }

        public NerveCacheKey CreateNeuronCacheKey(in CellWrap<Neuron, NeuronValue<TData>, TData, TLink> neuronWrap)
        {
            return nerve.CreateNeuronCacheKey(in neuronWrap.RefData);
        }

        public NerveCacheKey CreateNeuronCacheKey(in TData data)
        {
            return NerveCacheKey.Create(in data);
        }
    }

    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public bool TryGetNeuronConnectionCacheCore(in NerveCacheKey cacheKey,
            [NotNullWhen(true)] out DataOffset? offset)
        {
            return nerve.ConnectionSectionCache.TryGet(in cacheKey, out offset);
        }

        public void SetNeuronConnectionCache(in Neuron from,
            in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            nerve.SetNeuronConnectionCacheCore(
                nerve.CreateNeuronConnectionCacheKey(in from, in connectionWrap),
                connectionWrap.RefCell.Offset);
        }

        public void SetNeuronConnectionCacheCore(in NerveCacheKey cacheKey, in DataOffset offset)
        {
            nerve.ConnectionSectionCache.Set(in cacheKey, offset);
        }

        public NerveCacheKey CreateNeuronConnectionCacheKey(in Neuron from,
            in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            return nerve.CreateNeuronConnectionCacheKey(
                in from,
                connectionWrap.Neuron,
                in connectionWrap.RefLink);
        }

        public NerveCacheKey CreateNeuronConnectionCacheKey(in Neuron from,
            in Neuron to,
            in TLink link)
        {
            return NerveCacheKey.Create(
                from.Offset,
                to.Offset,
                in link);
        }
    }

    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public bool TryGetConnectionCache(
            in Neuron from,
            in Neuron to,
            in TLink link,
            in Connection previous,
            [NotNullWhen(true)] out Connection? connection)
        {
            if (nerve.TryGetConnectionCacheCore(
                    nerve.CreateConnectionCacheKey(in from, in to, in link, in previous),
                    out var offset))
            {
                connection = new(offset.Value);
                return true;
            }

            connection = null;
            return false;
        }

        public bool TryGetConnectionCacheCore(in NerveCacheKey cacheKey, [NotNullWhen(true)] out DataOffset? offset)
        {
            return nerve.ConnectionSectionCache.TryGet(in cacheKey, out offset);
        }

        public void SetConnectionCache(in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            nerve.SetConnectionCacheCore(
                nerve.CreateConnectionCacheKey(in connectionWrap),
                connectionWrap.RefCell.Offset);
        }

        public void SetConnectionCacheCore(in NerveCacheKey cacheKey, in DataOffset offset)
        {
            nerve.ConnectionSectionCache.Set(in cacheKey, offset);
        }

        public NerveCacheKey CreateConnectionCacheKey(
            in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            return nerve.CreateConnectionCacheKey(
                connectionWrap.PreviousWrap!.Value.Neuron,
                connectionWrap.Neuron,
                in connectionWrap.RefLink,
                connectionWrap.Previous!.Value);
        }

        public NerveCacheKey CreateConnectionCacheKey(
            in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap,
            in Neuron to,
            in TLink link)
        {
            return nerve.CreateConnectionCacheKey(
                connectionWrap.Neuron,
                to,
                in link,
                connectionWrap.RefCell);
        }

        public NerveCacheKey CreateConnectionCacheKey(
            in Neuron from,
            in Neuron to,
            in TLink link,
            in Connection previous)
        {
            return NerveCacheKey.Create(from.Offset, to.Offset, in link, previous.Offset);
        }
    }
}