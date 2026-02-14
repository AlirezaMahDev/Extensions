namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparerHelperExtensions
{
    extension<T>(ComparerHelper<T>)
        where T : allows ref struct
    {
        public static int? NullUp(T? x, T? y) =>
            x is null
                ? y is null
                    ? 0
                    : -1
                : y is null
                    ? 1
                    : null;

        public static int NullUp(T? x, T? y, Comparison<T> comparison) =>
            ComparerHelper<T>.NullUp(x, y) ?? comparison(x!, y!);

        public static int? NullDown(T? x, T? y) =>
            x is null
                ? y is null
                    ? 0
                    : 1
                : y is null
                    ? -1
                    : null;

        public static int NullDown(T? x, T? y, Comparison<T> comparison) =>
            ComparerHelper<T>.NullDown(x, y) ?? comparison(x!, y!);
    }
}