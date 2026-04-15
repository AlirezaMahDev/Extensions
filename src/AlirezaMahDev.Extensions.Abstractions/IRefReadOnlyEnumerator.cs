namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefReadOnlyEnumerator<TRefReadOnlyEnumerator, T>
    where TRefReadOnlyEnumerator : IRefReadOnlyEnumerator<TRefReadOnlyEnumerator, T>, allows ref struct
{
    ref readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool MoveNext();
}