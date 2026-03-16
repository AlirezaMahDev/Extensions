namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparerExtensions
{
    extension<T>(Comparer<T>)
    {
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

        public static int NullUp(T? x, T? y, Comparison<T> comparison)
        {
            return Comparer<T>.NullUp(x, y) ?? comparison(x!, y!);
        }

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

        public static int NullDown(T? x, T? y, Comparison<T> comparison)
        {
            return Comparer<T>.NullDown(x, y) ?? comparison(x!, y!);
        }
    }
}