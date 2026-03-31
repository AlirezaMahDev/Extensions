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
        public bool TryGetNeuronCache(ref readonly TData data, [NotNullWhen(true)] out Neuron? neuron)
        {
            var cacheKey = nerve.CreateNeuronCacheKey(in data);
            if (nerve.TryGetNeuronCacheCore(in cacheKey, out var offset))
            {
                neuron = new(offset.Value);
                return true;
            }

            neuron = null;
            return false;
        }

        public bool TryGetNeuronCacheCore(ref readonly NerveCacheKey cacheKey,
            [NotNullWhen(true)] out DataOffset? offset)
        {
            return nerve.NeuronSectionCache.TryGet(in cacheKey, out offset);
        }

        public void TrySetNeuronCache(ref readonly CellWrap<NeuronValue<TData>, TData, TLink> neuronWrap)
        {
            var cacheKey = nerve.CreateNeuronCacheKey(in neuronWrap);
            nerve.TrySetNeuronCacheCore(in cacheKey, in neuronWrap.Location.Offset);
        }

        public void TrySetNeuronCacheCore(ref readonly NerveCacheKey cacheKey, ref readonly DataOffset offset)
        {
            nerve.NeuronSectionCache.TrySet(in cacheKey, in offset);
        }

        public NerveCacheKey CreateNeuronCacheKey(ref readonly CellWrap<NeuronValue<TData>, TData, TLink> neuronWrap)
        {
            using var neuron = neuronWrap.Location.ReadLock();
            return nerve.CreateNeuronCacheKey(in neuron.RefReadOnlyValue.Data);
        }

        public NerveCacheKey CreateNeuronCacheKey(ref readonly TData data)
        {
            return NerveCacheKey.Create(in data)
                ;
        }
    }

    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public bool TryGetNeuronConnectionCacheCore(ref readonly NerveCacheKey cacheKey,
            [NotNullWhen(true)] out DataOffset? offset)
        {
            return nerve.ConnectionSectionCache.TryGet(in cacheKey, out offset);
        }

        public void TrySetNeuronConnectionCache(ref readonly DataOffset from,
            ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            var cacheKey = nerve.CreateNeuronConnectionCacheKey(in from, in connectionWrap);
            nerve.TrySetNeuronConnectionCacheCore(
                in cacheKey,
                in connectionWrap.Location.Offset);
        }

        public void TrySetNeuronConnectionCacheCore(ref readonly NerveCacheKey cacheKey, ref readonly DataOffset offset)
        {
            nerve.ConnectionSectionCache.TrySet(in cacheKey, in offset);
        }

        public NerveCacheKey CreateNeuronConnectionCacheKey(ref readonly DataOffset from,
            ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            using var connectionValue = connectionWrap.Location.ReadLock();
            return nerve.CreateNeuronConnectionCacheKey(
                    in from,
                    in connectionValue.RefReadOnlyValue.Neuron.Offset,
                    in connectionValue.RefReadOnlyValue.Link)
                ;
        }

        public NerveCacheKey CreateNeuronConnectionCacheKey(ref readonly DataOffset from,
            ref readonly DataOffset to,
            ref readonly TLink link)
        {
            return NerveCacheKey.Create(
                    in from,
                    in to,
                    in link)
                ;
        }
    }

    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public bool TryGetConnectionCache(
            ref readonly Neuron fromNeuron,
            ref readonly Neuron toNeuron,
            ref readonly TLink link,
            ref readonly Connection previousConnection,
            [NotNullWhen(true)] out Connection? connection)
        {
            var cacheKey = nerve.CreateConnectionCacheKey(in fromNeuron,
                in toNeuron,
                in link,
                in previousConnection.Offset);
            if (nerve.TryGetConnectionCacheCore(
                    in cacheKey,
                    out var offset))
            {
                connection = new(offset.Value);
                return true;
            }

            connection = null;
            return false;
        }

        public bool TryGetConnectionCacheCore(ref readonly NerveCacheKey cacheKey,
            [NotNullWhen(true)] out DataOffset? offset)
        {
            return nerve.ConnectionSectionCache.TryGet(in cacheKey, out offset);
        }

        public void TrySetConnectionCache(ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            var cacheKey = nerve.CreateConnectionCacheKey(in connectionWrap);
            nerve.TrySetConnectionCacheCore(in cacheKey, in connectionWrap.Location.Offset);
        }

        public void TrySetConnectionCacheCore(ref readonly NerveCacheKey cacheKey, ref readonly DataOffset offset)
        {
            nerve.ConnectionSectionCache.TrySet(in cacheKey, in offset);
        }

        public NerveCacheKey CreateConnectionCacheKey(
            ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            using var connectionValue = connectionWrap.Location.ReadLock();
            using var previousWrap = connectionWrap.PreviousWrap.Location.ReadLock();
            return nerve.CreateConnectionCacheKey(
                in previousWrap.RefReadOnlyValue.Neuron,
                in connectionValue.RefReadOnlyValue.Neuron,
                in connectionValue.RefReadOnlyValue.Link,
                in connectionValue.RefReadOnlyValue.Previous.Offset);
        }

        public NerveCacheKey CreateConnectionCacheKey(
            ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> connectionWrap,
            ref readonly Neuron toNeuron,
            ref readonly TLink link)
        {
            using var connectionValue = connectionWrap.Location.ReadLock();
            return nerve.CreateConnectionCacheKey(
                in connectionValue.RefReadOnlyValue.Neuron,
                in toNeuron,
                in link,
                in connectionWrap.Location.Offset);
        }

        public NerveCacheKey CreateConnectionCacheKey(
            ref readonly Neuron fromNeuron,
            ref readonly Neuron toNeuron,
            ref readonly TLink link,
            ref readonly DataOffset previousConnection)
        {
            return NerveCacheKey.Create(in fromNeuron,
                in toNeuron,
                in link,
                in previousConnection);
        }
    }
}