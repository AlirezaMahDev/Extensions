namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct RefReadOnlyBlock<TSelf, T>(TSelf self) : IRefReadOnlyBlock<RefReadOnlyBlock<TSelf, T>, T>
    where TSelf : IRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _start;

    public readonly ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            if (index >= Length)
                throw new IndexOutOfRangeException($"{index} >= {Length}");
            return ref _self[_start + index];
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    } = self.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly RefReadOnlyIndexableEnumerator<RefReadOnlyBlock<TSelf, T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefReadOnlyBlock<TSelf, T> Slice(int start, int length)
    {
        return start + length > Length
            ? throw new IndexOutOfRangeException($"{start + length} > {Length}")
            : (this with
            {
                _start = _start + start,
                Length = length
            });
    }
}