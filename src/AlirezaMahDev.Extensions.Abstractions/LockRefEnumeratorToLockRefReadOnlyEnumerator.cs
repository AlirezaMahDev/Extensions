namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct LockRefEnumeratorToLockRefReadOnlyEnumerator<TLockRefEnumerator, T>(TLockRefEnumerator refEnumerator)
    : ILockRefReadOnlyEnumerator<LockRefEnumeratorToLockRefReadOnlyEnumerator<TLockRefEnumerator, T>, T>
    where TLockRefEnumerator : ILockRefEnumerator<TLockRefEnumerator, T>, allows ref struct
{
    private readonly TLockRefEnumerator _refEnumerator = refEnumerator;
    public LockRefReadOnlyItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _refEnumerator.Current;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext() => _refEnumerator.MoveNext();
}