namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveCacheExtensions
{
    extension(INerveCache nerveCache)
    {
        public INerveCacheSection NeuronSection => nerveCache.GetOrAdd("n");
        public INerveCacheSection ConnectionSection => nerveCache.GetOrAdd("c");
        public INerveCacheSection LastLoadedConnection => nerveCache.GetOrAdd("l");
    }
}