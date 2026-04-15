namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefReadOnlyEnumerator<TLockRefReadOnlyEnumerator, T>
    where TLockRefReadOnlyEnumerator : ILockRefReadOnlyEnumerator<TLockRefReadOnlyEnumerator, T>, allows ref struct
{
    LockRefReadOnlyItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool MoveNext();
}