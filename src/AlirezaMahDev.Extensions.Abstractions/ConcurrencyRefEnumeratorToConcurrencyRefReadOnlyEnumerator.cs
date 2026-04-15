namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct ConcurrencyRefEnumeratorToConcurrencyRefReadOnlyEnumerator<TConcurrencyRefEnumerator, T>(TConcurrencyRefEnumerator refEnumerator)
    : ILockRefReadOnlyEnumerator<ConcurrencyRefEnumeratorToConcurrencyRefReadOnlyEnumerator<TConcurrencyRefEnumerator, T>, T>
    where TConcurrencyRefEnumerator : ILockRefEnumerator<TConcurrencyRefEnumerator, T>, allows ref struct
{
    private readonly TConcurrencyRefEnumerator _refEnumerator = refEnumerator;
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