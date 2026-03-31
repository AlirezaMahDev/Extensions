namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparerExtensions
{
    extension<T>(Comparer<T>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int? NullUp(T? x, T? y)
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
        public static int NullUp(T? x, T? y, ScopedRefReadOnlyComparison<T> comparison)
        {
            return Comparer<T>.NullUp(x, y) ?? comparison(ref x!, ref y!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int? NullDown(T? x, T? y)
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
        public static int NullDown(T? x, T? y, ScopedRefReadOnlyComparison<T> comparison)
        {
            return Comparer<T>.NullDown(x, y) ?? comparison(ref x!, ref y!);
        }
    }
}