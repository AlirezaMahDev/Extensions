namespace AlirezaMahDev.Extensions.Abstractions;

public ref struct LockRefReadOnlyIndexableEnumerator<TSelf, T>(TSelf self) : ILockRefReadOnlyEnumerator<LockRefReadOnlyIndexableEnumerator<TSelf, T>, T>
    where TSelf : ILockRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    private int _index = -1;
    private readonly TSelf _self = self;

    public readonly LockRefReadOnlyItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self[_index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        _index++;
        return _index < _self.Length;
    }
}