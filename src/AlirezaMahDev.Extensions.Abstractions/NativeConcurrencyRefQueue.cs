namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public sealed class NativeConcurrencyRefQueue<T>(int capacity, bool init) : IConcurrencyRefQueue<NativeConcurrencyRefQueue<T>, T>, IDisposable
    where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefQueue<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    private NativeLockRefQueue<ConcurrencyIndex> _queue = NativeLockRefQueue<ConcurrencyIndex>.Create(capacity, init);
    private NativeConcurrencyRefBag<T> _bag = NativeConcurrencyRefBag<T>.Create(capacity, init);

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _bag.Length;
        }
    }

    public LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _bag[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPeek(out ConcurrencyRefIndexableItem<NativeConcurrencyRefQueue<T>, T> result)
    {
        if (!_queue.TryPeek(out var index)!.Value)
        {
            result = default;
            return false;
        }

        result = new(this, index.Value.CopyValue);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryDequeue(out T result)
    {
        if (!_queue.TryDequeue(out var index)!.Value)
        {
            result = default;
            return false;
        }

        return _bag.TryRemove(index, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryEnqueue(in T value)
    {
        return _queue.TryEnqueue(_bag.TryAdd(in value))!.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _queue.Dispose();
        _bag.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        _queue.Clean();
        _bag.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefQueue<T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingLength()
    {
        return _bag.GetShardingLength();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingItemsLength(int shardingIndex)
    {
        return _bag.GetShardingItemsLength(shardingIndex);
    }
}