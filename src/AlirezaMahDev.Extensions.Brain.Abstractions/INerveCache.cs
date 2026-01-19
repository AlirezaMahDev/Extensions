namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerveCache
{
    INerveCacheSection GetOrAdd(string key);
}