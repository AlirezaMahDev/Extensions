namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefReadOnlyBlock<TSelf, T> : ILockRefReadOnlyIndexable<TSelf, T>
    where TSelf : ILockRefReadOnlyBlock<TSelf, T>, allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    TSelf Slice(int start, int length);
}