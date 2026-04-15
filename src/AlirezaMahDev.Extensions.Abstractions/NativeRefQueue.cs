namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public struct NativeRefQueue<T>(int capacity = 1, bool init = false)
    : IRefQueue<NativeRefQueue<T>, T>, IDisposable
    where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeRefQueue<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    private NativeRefList<T> _list = new(capacity, init);

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _list.Length;
        }
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _list[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _list.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryDequeue(out T result)
    {
        if (_list.Length == 0)
        {
            result = default;
            return false;
        }

        _list.Remove(0, out result);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryEnqueue(in T value)
    {
        return _list.Add(in value) != -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPeek(out RefIndexableItem<NativeRefQueue<T>, T> result)
    {
        if (_list.Length == 0)
        {
            result = default;
            return false;
        }

        result = new(this, 0);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefIndexableEnumerator<NativeRefQueue<T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        _list.Clean();
    }
}