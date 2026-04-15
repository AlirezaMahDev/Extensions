namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct RefReadOnlyIndexableEnumerator<TSelf, T>(TSelf self)
    : IRefReadOnlyEnumerator<RefReadOnlyIndexableEnumerator<TSelf, T>, T>
    where TSelf : IRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    private int _index = -1;
    private readonly TSelf _self = self;

    public readonly ref readonly T Current
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