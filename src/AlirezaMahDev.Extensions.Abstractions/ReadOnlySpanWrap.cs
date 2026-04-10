namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct ReadOnlySpanWrap<T>(ReadOnlySpan<T> readOnlySpan)
    : IRefReadOnlyBlock<ReadOnlySpanWrap<T>, T>
{
    private readonly ReadOnlySpan<T> _readOnlySpan = readOnlySpan;

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _readOnlySpan.Length;
        }
    }

    public ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _readOnlySpan[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReadOnlySpanWrap<T> Slice(int start, int length) =>
        new(_readOnlySpan.Slice(start, length));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefReadOnlyIndexableEnumerator<ReadOnlySpanWrap<T>, T> GetEnumerator() => new(this);
}