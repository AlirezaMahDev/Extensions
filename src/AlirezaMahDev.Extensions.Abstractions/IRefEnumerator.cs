namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefEnumerator<TSelf, T> : IRefReadOnlyEnumerator<TSelf, T>
    where TSelf : IRefEnumerator<TSelf, T>, allows ref struct
{
    ref readonly T IRefReadOnlyEnumerator<TSelf, T>.Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Current;
    }

    new ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}