namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparerHelperExtensions
{
    extension<T>(ComparerHelper<T>)
        where T : allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int? NullUp(scoped ref readonly T? x, scoped ref readonly T? y)
        {
            return x is null
                ? y is null
                    ? 0
                    : -1
                : y is null
                    ? 1
                    : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int NullUp(scoped ref readonly T? x,
            scoped ref readonly T? y,
            ScopedRefReadOnlyComparison<T> comparison)
        {
            return ComparerHelper<T>.NullUp(in x, in y) ?? comparison(in x!, in y!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int? NullDown(scoped ref readonly T? x, scoped ref readonly T? y)
        {
            return x is null
                ? y is null
                    ? 0
                    : 1
                : y is null
                    ? -1
                    : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int NullDown(scoped ref readonly T? x,
            scoped ref readonly T? y,
            ScopedRefReadOnlyComparison<T> comparison)
        {
            return ComparerHelper<T>.NullDown(in x, in y) ?? comparison(in x!, in y!);
        }
    }
}