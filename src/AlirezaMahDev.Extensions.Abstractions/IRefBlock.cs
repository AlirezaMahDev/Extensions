namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefBlock<TSelf, T> : IRefIndexable<TSelf, T>
    where TSelf : IRefBlock<TSelf, T>, allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    TSelf Slice(int start, int length);
}