namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ValueTypeExtensions
{
    extension<T>(T value)
        where T : struct
    {
        public T? NullWhenDefault() =>
            EqualityComparer<T>.Default.Equals(value, default) ? null : value;
    }

    extension<TSource, TDestination>(TSource? source)
        where TSource : struct
        where TDestination : class
    {
        public TDestination? Convert(Func<TSource, TDestination> func, TDestination? nullValue = null) =>
            source.HasValue ? func(source.Value) : nullValue;
    }

    extension<TSource, TDestination>(TSource? source)
        where TSource : struct
        where TDestination : struct
    {
        public TDestination? Convert(Func<TSource, TDestination> func, TDestination? nullValue = null) =>
            source.HasValue ? func(source.Value) : nullValue;
    }
}