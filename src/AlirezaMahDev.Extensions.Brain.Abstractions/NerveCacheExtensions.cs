namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveCacheExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public INerveCacheSection NeuronSectionCache => nerve.Cache.GetOrAdd("n");
        public INerveCacheSection ConnectionSectionCache => nerve.Cache.GetOrAdd("c");
        public INerveCacheSection NextUnloadItem => nerve.Cache.GetOrAdd("l");
    }

    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public bool TryGetNeuronCache(ref readonly TData data, out Neuron neuron)
        {
            var cacheKey = INerve<TData, TLink>.CreateNeuronCacheKey(in data);
            return nerve.TryGetNeuronCacheCore(in cacheKey, out neuron);
        }

        public bool TryGetNeuronCacheCore(ref readonly NerveCacheKey cacheKey, out Neuron neuron)
        {
            if (nerve.NeuronSectionCache.TryGet(in cacheKey, out var offset))
            {
                neuron = new(offset.Value);
                return true;
            }

            neuron = default;
            return false;
        }

        public bool TrySetNeuronCache(ref readonly TData data, ref readonly Neuron neuron)
        {
            var cacheKey = INerve<TData, TLink>.CreateNeuronCacheKey(in data);
            return nerve.TrySetNeuronCacheCore(in cacheKey, in neuron);
        }

        public bool TrySetNeuronCacheCore(ref readonly NerveCacheKey cacheKey, ref readonly Neuron neuron)
        {
            return nerve.NeuronSectionCache.TrySet(in cacheKey, in neuron.Offset);
        }

        public static NerveCacheKey CreateNeuronCacheKey(ref readonly TData data)
        {
            return NerveCacheKey.Create(in data);
        }
    }

    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public bool TryGetConnectionCache(
            ref readonly Connection parent,
            ref readonly TLink link,
            ref readonly Neuron neuron,
            out Connection connection)
        {
            var cacheKey = INerve<TData, TLink>.CreateConnectionCacheKey(in parent, in link, in neuron);
            return nerve.TryGetConnectionCacheCore(in cacheKey, out connection);
        }

        public bool TryGetConnectionCacheCore(ref readonly NerveCacheKey cacheKey, out Connection connection)
        {
            if (nerve.ConnectionSectionCache.TryGet(in cacheKey, out var offset))
            {
                connection = new(offset.Value);
                return true;
            }

            connection = default;
            return false;
        }

        public bool TrySetConnectionCache(
            ref readonly Connection parent,
            ref readonly TLink link,
            ref readonly Neuron neuron,
            ref readonly Connection connection)
        {
            var cacheKey = INerve<TData, TLink>.CreateConnectionCacheKey(in parent, in link, in neuron);
            return nerve.TrySetConnectionCacheCore(in cacheKey, in connection);
        }

        public bool TrySetConnectionCacheCore(ref readonly NerveCacheKey cacheKey, ref readonly Connection connection)
        {
            return nerve.ConnectionSectionCache.TrySet(in cacheKey, in connection.Offset);
        }

        public static NerveCacheKey CreateConnectionCacheKey(
            ref readonly Connection parent,
            ref readonly TLink link,
            ref readonly Neuron neuron)
        {
            return NerveCacheKey.Create(in parent, in link, in neuron);
        }
    }
}