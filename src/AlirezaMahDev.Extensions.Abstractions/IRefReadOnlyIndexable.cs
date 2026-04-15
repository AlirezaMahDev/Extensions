namespace AlirezaMahDev.Extensions.Abstractions;

public interface
    IRefReadOnlyIndexable<TSelf, T> : IRefReadOnlyCountable<TSelf, T, RefReadOnlyIndexableEnumerator<TSelf, T>>
    where TSelf : IRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}