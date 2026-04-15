namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct RefBlock<TSelf, T>(TSelf self) : IRefBlock<RefBlock<TSelf, T>, T>
    where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _start;

    public readonly ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            if (index >= Length)
                throw new IndexOutOfRangeException($"${index} >= {Length}");
            return ref _self[_start + index];
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    } = self.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly RefIndexableEnumerator<RefBlock<TSelf, T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefBlock<TSelf, T> Slice(int start, int length)
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