namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct SpanValue<T>
    where T : struct
{
    private readonly Span<T> _span;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SpanValue(ref T value)
    {
        _span = MemoryMarshal.CreateSpan(ref value, 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SpanValue(Span<T> span)
    {
        _span = span[..1];
    }

    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => !_span.IsEmpty;
    }

    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref MemoryMarshal.GetReference(_span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator Span<T>(SpanValue<T> spanValue)
    {
        return spanValue._span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlySpan<T>(SpanValue<T> spanValue)
    {
        return spanValue._span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlySpanValue<T>(SpanValue<T> spanValue)
    {
        return new(spanValue);
    }
}