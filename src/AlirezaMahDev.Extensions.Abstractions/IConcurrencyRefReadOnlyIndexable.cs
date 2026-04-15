namespace AlirezaMahDev.Extensions.Abstractions;

public interface IConcurrencyRefReadOnlyIndexable<TSelf, T> : ILockRefReadOnlyIndexable<TSelf, T>
    where TSelf : IConcurrencyRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    LockRefReadOnlyItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}