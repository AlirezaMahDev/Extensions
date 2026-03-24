namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparerHelperExtensions
{
    extension<T>(ComparerHelper<T>)
        where T : allows ref struct
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
        public static int NullUp(T? x, T? y, Comparison<T> comparison)
        {
            return ComparerHelper<T>.NullUp(x, y) ?? comparison(x!, y!);
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
        public static int NullDown(T? x, T? y, Comparison<T> comparison)
        {
            return ComparerHelper<T>.NullDown(x, y) ?? comparison(x!, y!);
        }
    }
}