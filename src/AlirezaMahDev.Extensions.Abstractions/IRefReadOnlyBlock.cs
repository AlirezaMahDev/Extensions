namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefReadOnlyBlock<TSelf, T, out TEnumerator> : IRefReadOnlyEnumerable<TSelf, T, TEnumerator>,
    IRefLength
    where TSelf : IRefReadOnlyBlock<TSelf, T, TEnumerator>, allows ref struct
    where TEnumerator : IRefReadOnlyEnumerator<TEnumerator, T>, allows ref struct

{
    ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    TSelf Slice(int start, int length);
}