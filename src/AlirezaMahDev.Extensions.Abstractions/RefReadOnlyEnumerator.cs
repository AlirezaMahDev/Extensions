namespace AlirezaMahDev.Extensions.Abstractions;

public ref struct RefReadOnlyEnumerator<TRefReadOnlyBlock, T>(TRefReadOnlyBlock block)
    : IRefReadOnlyEnumerator<RefReadOnlyEnumerator<TRefReadOnlyBlock, T>, T>
    where TRefReadOnlyBlock : IRefReadOnlyBlock<TRefReadOnlyBlock, T, RefReadOnlyEnumerator<TRefReadOnlyBlock, T>>,
    allows ref struct
{
    private int _index = -1;
    private TRefReadOnlyBlock _block = block;

    public ref readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _block[_index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        if (_index < _block.Length - 1)
        {
            _index++;
            return true;
        }

        return false;
    }
}