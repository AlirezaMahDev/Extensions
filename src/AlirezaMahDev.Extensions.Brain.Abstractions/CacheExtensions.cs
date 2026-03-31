namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CacheExtensions
{
    extension(scoped ref readonly Cache cache)
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLocationWrap<CacheValue, DataEmptyWrap> NewWrap(IDataAccess access)
        {
            DataLocation<CacheValue>.Read(access, cache.Offset, out var location);
            return new(access, location, DataEmptyWrap.Default);
        }
    }
}