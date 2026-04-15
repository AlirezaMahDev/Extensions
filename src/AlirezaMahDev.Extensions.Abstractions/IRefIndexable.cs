namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefIndexable<TSelf, T> : IRefCountable<TSelf, T, RefIndexableEnumerator<TSelf, T>>
    where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}