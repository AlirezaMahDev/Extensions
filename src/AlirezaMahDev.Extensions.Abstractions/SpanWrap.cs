namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct SpanWrap<T>(Span<T> span) : IRefBlock<SpanWrap<T>, T, RefEnumerator<SpanWrap<T>, T>>
{
    private readonly Span<T> _span = span;

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _span.Length;
        }
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _span[index];
        }
    }

    ref readonly T
        IRefReadOnlyBlock<SpanWrap<T>, T, RefEnumeratorToRefReadOnlyEnumerator<RefEnumerator<SpanWrap<T>, T>, T>>.
        this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref this[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SpanWrap<T> Slice(int start, int length) =>
        new(_span.Slice(start, length));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefEnumerator<SpanWrap<T>, T> GetEnumerator() => new(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlySpanWrap<T>(SpanWrap<T> span) =>
        new(span._span);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    RefEnumeratorToRefReadOnlyEnumerator<RefEnumerator<SpanWrap<T>, T>, T> IRefReadOnlyEnumerable<SpanWrap<T>, T,
        RefEnumeratorToRefReadOnlyEnumerator<RefEnumerator<SpanWrap<T>, T>, T>>.GetEnumerator()
    {
        return new(GetEnumerator());
    }
}