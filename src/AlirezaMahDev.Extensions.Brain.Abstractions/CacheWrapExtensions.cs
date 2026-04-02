namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CacheWrapExtensions
{
    extension(scoped in DataWrap<CacheValue, DataEmptyWrap> wrap)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Clear() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Set(scoped ref readonly UInt128 key, scoped ref readonly DataOffset value) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Set(scoped ref readonly UInt128 key, ScopedRefReadOnlyFunc<UInt128, DataOffset> value) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TrySet(scoped ref readonly UInt128 key, scoped ref readonly DataOffset value) { return false; }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TrySet(scoped ref readonly UInt128 key, ScopedRefReadOnlyFunc<UInt128, DataOffset> value)
        {
            return false;
        }

        public RefReadOnlyOptional<DataOffset> Get(scoped ref readonly UInt128 key) { return default; }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryGet(scoped ref readonly UInt128 key, out DataOffset value)
        {
            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryRemove(scoped ref readonly UInt128 key, out DataOffset value)
        {
            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public RefReadOnlyOptional<DataOffset> GetOrSet(scoped ref readonly UInt128 key,
            scoped ref readonly DataOffset value)
        {
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public RefReadOnlyOptional<DataOffset> GetOrSet(scoped ref readonly UInt128 key,
            ScopedRefReadOnlyFunc<UInt128, DataOffset> value)
        {
            return default;
        }
    }
}