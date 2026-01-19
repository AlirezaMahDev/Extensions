using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.Brain.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class NerveCache : INerveCache
{
    private readonly ConcurrentDictionary<string, NerveCacheSection> _cache = [];

    public INerveCacheSection GetOrAdd(string key) =>
        _cache.GetOrAdd(key, static _ => new());
}