namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefIndexable<TSelf, T> : ILockRefCountable<TSelf, T, LockRefIndexableEnumerator<TSelf, T>>, ILockerStatus
    where TSelf : ILockRefIndexable<TSelf, T>, allows ref struct
{
    LockRefItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}