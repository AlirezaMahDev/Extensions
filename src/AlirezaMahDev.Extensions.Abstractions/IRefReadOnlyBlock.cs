namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefReadOnlyBlock<TSelf, T> : IRefReadOnlyIndexable<TSelf, T>
    where TSelf : IRefReadOnlyBlock<TSelf, T>, allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    TSelf Slice(int start, int length);
}