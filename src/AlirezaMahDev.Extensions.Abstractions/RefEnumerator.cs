namespace AlirezaMahDev.Extensions.Abstractions;

public ref struct RefEnumerator<TRefBlock, T>(TRefBlock block) : IRefEnumerator<RefEnumerator<TRefBlock, T>, T>
    where TRefBlock : IRefBlock<TRefBlock, T, RefEnumerator<TRefBlock, T>>, allows ref struct
{
    private int _index = -1;
    private TRefBlock _block = block;

    ref readonly T IRefReadOnlyEnumerator<RefEnumerator<TRefBlock, T>, T>.Current => ref Current;

    public ref T Current
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