namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CacheBlockValueExtensions
{
    extension(in DataWrap<CacheBlockValue, DataEmptyWrap> wrap)
    {
        public Optional<DataWrap<CacheBlockValue, DataEmptyWrap>> NextWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Location.ReadLock((scoped ref readonly value, scoped ref readonly access) =>
                    value.Next.AsOptionalLocation<CacheBlockValue>(access)
                        .WhenNotNull((scoped ref readonly next, scoped ref readonly access) => next.EmptyWrap(access),
                            in access),
                wrap.Access);
        }
    }
}