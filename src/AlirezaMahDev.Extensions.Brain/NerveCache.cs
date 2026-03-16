using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.Brain.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

internal class NerveCache : INerveCache
{
    private readonly ConcurrentDictionary<string, NerveCacheSection> _cache = [];

    public INerveCacheSection GetOrAdd(string key)
    {
        return _cache.GetOrAdd(key, static _ => new());
    }
}