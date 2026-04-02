namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefBlock<TSelf, T, TEnumerator> : IRefEnumerable<TSelf, T, TEnumerator>,
    IRefReadOnlyBlock<TSelf, T, RefEnumeratorToRefReadOnlyEnumerator<TEnumerator, T>>
    where TSelf : IRefBlock<TSelf, T, TEnumerator>, allows ref struct
    where TEnumerator : IRefEnumerator<TEnumerator, T>, allows ref struct
{
    ref readonly T IRefReadOnlyBlock<TSelf, T, RefEnumeratorToRefReadOnlyEnumerator<TEnumerator, T>>.this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref this[index];
        }
    }

    new ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}