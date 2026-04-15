namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefBlock<TSelf, T> : ILockRefIndexable<TSelf, T>
    where TSelf : ILockRefBlock<TSelf, T>, allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    TSelf Slice(int start, int length);
}