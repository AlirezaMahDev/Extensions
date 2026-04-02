namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefReadOnlyEnumerator<TSelf, T>
    where TSelf : IRefReadOnlyEnumerator<TSelf, T>, allows ref struct
{
    ref readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool MoveNext();
}