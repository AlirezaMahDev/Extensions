namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct RefIndexableEnumerator<TSelf, T>(TSelf self) : IRefEnumerator<RefIndexableEnumerator<TSelf, T>, T>
    where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    private int _index = -1;
    private readonly TSelf _self = self;

    public readonly ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _self[_index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        _index++;
        return _index < _self.Length;
    }
}