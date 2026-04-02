namespace AlirezaMahDev.Extensions.Abstractions;

public static class ReadOnlySpanWrapExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ReadOnlySpanWrap<T> AsReadOnlySpanWrap<T>(this ReadOnlySpan<T> readOnlySpan) =>
        new(readOnlySpan);
}