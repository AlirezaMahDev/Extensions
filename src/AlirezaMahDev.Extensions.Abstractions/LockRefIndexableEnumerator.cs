using System.Numerics;

namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct LockRefIndexableEnumerator<TSelf, T>(TSelf self) : ILockRefEnumerator<LockRefIndexableEnumerator<TSelf, T>, T>
where TSelf : ILockRefIndexable<TSelf, T>, allows ref struct
{
    private int _index = -1;
    private readonly TSelf _self = self;

    public readonly LockRefItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self[_index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        _index++;
        return _index < _self.Length;
    }
}