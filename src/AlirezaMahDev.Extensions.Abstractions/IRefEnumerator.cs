namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefEnumerator<TRefEnumerator, T>
    where TRefEnumerator : IRefEnumerator<TRefEnumerator, T>, allows ref struct
{
    ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool MoveNext();
}