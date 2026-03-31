namespace AlirezaMahDev.Extensions.Brain;

internal sealed class NerveCache : INerveCache, IDisposable
{
    private readonly ConcurrentDictionary<string, NerveCacheSection> _cache = [];

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        foreach (var section in _cache.Values)
        {
            section.Dispose();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public INerveCacheSection GetOrAdd(string key)
    {
        return _cache.GetOrAdd(key, static _ => new());
    }
}