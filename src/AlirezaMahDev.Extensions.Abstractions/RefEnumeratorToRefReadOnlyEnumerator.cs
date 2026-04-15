namespace AlirezaMahDev.Extensions.Abstractions;

public readonly ref struct RefEnumeratorToRefReadOnlyEnumerator<TRefEnumerator, T>(TRefEnumerator refEnumerator)
    : IRefReadOnlyEnumerator<RefEnumeratorToRefReadOnlyEnumerator<TRefEnumerator, T>, T>
    where TRefEnumerator : IRefEnumerator<TRefEnumerator, T>, allows ref struct
{
    private readonly TRefEnumerator _refEnumerator = refEnumerator;

    public ref readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _refEnumerator.Current;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext() => _refEnumerator.MoveNext();
}