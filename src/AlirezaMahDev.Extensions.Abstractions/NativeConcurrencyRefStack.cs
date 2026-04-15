namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public sealed class NativeConcurrencyRefStack<T>(int capacity, bool init) : IConcurrencyRefStack<NativeConcurrencyRefStack<T>, T>, IDisposable
    where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefStack<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    private NativeLockRefStack<ConcurrencyIndex> _stack = NativeLockRefStack<ConcurrencyIndex>.Create(capacity, init);
    private NativeConcurrencyRefBag<T> _bag = NativeConcurrencyRefBag<T>.Create(capacity, init);

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {

            return _stack.Length;
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
    public void Dispose()
    {

        _stack.Dispose();
        _bag.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPop(out T result)
    {

        if (!_stack.TryPop(out var index)!.Value)
        {
            result = default;
            return false;
        }

        return _bag.TryRemove(index, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPeek(out ConcurrencyRefIndexableItem<NativeConcurrencyRefStack<T>, T> result)
    {
        if (!_stack.TryPeek(out var index)!.Value)
        {
            result = default;
            return false;
        }

        result = new(this, index.Value.CopyValue);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPush(in T value)
    {
        return _stack.TryPush(_bag.TryAdd(in value)!)!.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        _stack.Clean();
        _bag.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefStack<T>, T> GetEnumerator()
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