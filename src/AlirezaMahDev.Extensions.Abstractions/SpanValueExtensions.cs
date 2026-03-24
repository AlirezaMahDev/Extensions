namespace AlirezaMahDev.Extensions.Abstractions;

public static class SpanValueExtensions
{
    extension<T>(ref T t)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public SpanValue<T> AsSpanValue()
        {
            return new(ref t);
        }
    }

    extension<T>(SpanValue<T> spanValue)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpanValue<T> AsReadOnlySpanValue()
        {
            return spanValue;
        }
    }
}