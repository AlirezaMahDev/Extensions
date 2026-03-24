namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ValueTypeExtensions
{
    extension<T>(T value)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public T? NullWhenDefault()
        {
            return EqualityComparer<T>.Default.Equals(value, default) ? null : value;
        }
    }

    extension<TSource, TDestination>(TSource? source)
        where TSource : struct
        where TDestination : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TDestination? Convert(Func<TSource, TDestination> func, TDestination? nullValue = null)
        {
            return source.HasValue ? func(source.Value) : nullValue;
        }
    }

    extension<TSource, TDestination>(TSource? source)
        where TSource : struct
        where TDestination : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TDestination? Convert(Func<TSource, TDestination> func, TDestination? nullValue = null)
        {
            return source.HasValue ? func(source.Value) : nullValue;
        }
    }
}