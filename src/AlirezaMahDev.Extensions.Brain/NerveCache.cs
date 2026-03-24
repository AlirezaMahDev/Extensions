namespace AlirezaMahDev.Extensions.Brain;

internal class NerveCache : INerveCache
{
    private readonly ConcurrentDictionary<string, NerveCacheSection> _cache = [];

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public INerveCacheSection GetOrAdd(string key)
    {
        return _cache.GetOrAdd(key, static _ => new());
    }
}