namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefReadOnlyIndexable<TSelf, T> : ILockRefReadOnlyCountable<TSelf, T, LockRefReadOnlyIndexableEnumerator<TSelf, T>>
    where TSelf : ILockRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    LockRefReadOnlyItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}