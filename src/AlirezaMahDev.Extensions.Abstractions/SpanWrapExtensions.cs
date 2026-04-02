namespace AlirezaMahDev.Extensions.Abstractions;

public static class SpanWrapExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SpanWrap<T> AsSpanWrap<T>(this Span<T> span) =>
        new(span);
}