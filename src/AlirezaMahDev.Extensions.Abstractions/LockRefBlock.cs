namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct LockRefBlock<TSelf, T>(TSelf self) : ILockRefBlock<LockRefBlock<TSelf, T>, T>
    where TSelf : ILockRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _start;
    private int _length;

    public readonly LockRefItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return index >= _length ? throw new IndexOutOfRangeException($"${index} >= {_length}") : _self[_start + index];
        }
    }
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _length;
        }
    }

    public readonly ReaderWriterLockerState LockerState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self.LockerState;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly LockRefIndexableEnumerator<LockRefBlock<TSelf, T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public LockRefBlock<TSelf, T> Slice(int start, int length)
    {
        return start + length > _length
            ? throw new IndexOutOfRangeException($"{start + length} > {_length}")
            : (this with { _start = _start + start, _length = length });
    }
}