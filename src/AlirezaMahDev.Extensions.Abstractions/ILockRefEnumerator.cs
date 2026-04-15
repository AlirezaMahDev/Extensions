namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefEnumerator<TLockRefEnumerator, T>
    where TLockRefEnumerator : ILockRefEnumerator<TLockRefEnumerator, T>, allows ref struct
{
    LockRefItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool MoveNext();
}