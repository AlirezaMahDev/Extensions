namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Auto)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct CacheBlock(ref readonly DataWrap<CacheBlockValue, DataEmptyWrap> wrap)
    : IRefList<CacheBlock, CacheItem, RefEnumerator<CacheBlock, CacheItem>>,
        IRefReadOnlyBlock<CacheBlock, CacheItem,
            RefEnumeratorToRefReadOnlyEnumerator<RefEnumerator<CacheBlock, CacheItem>, CacheItem>>
{
    private readonly ref readonly DataWrap<CacheBlockValue, DataEmptyWrap> _wrap = ref wrap;
    private readonly Index _start = 0;
    private readonly Index _end = ^0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private CacheBlock(ref readonly DataWrap<CacheBlockValue, DataEmptyWrap> wrap, Index start, Index end) :
        this(in wrap)
    {
        _start = start;
        _end = end;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Span<CacheItem> GetSpan(scoped ref readonly CacheBlockValue value)
    {
        var offset = _wrap
            .Wrap(x => x.Storage())
            .GetOrCreateData(value.Capacity * CacheItem.Size);
        return MemoryMarshal.Cast<byte, CacheItem>(_wrap.Access.GetSpan(in offset));
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => _wrap.Location.ReadLock((scoped ref readonly value) => value.Count);
    }

    ref readonly CacheItem
        IRefReadOnlyBlock<CacheBlock, CacheItem,
            RefEnumeratorToRefReadOnlyEnumerator<RefEnumerator<CacheBlock, CacheItem>, CacheItem>>.this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref this[index];
    }

    public ref CacheItem this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _wrap.Location.ReadLock();
            return ref GetSpan(in @lock.RefReadOnlyValue)[_start.._end][index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public CacheBlock Slice(int start, int length)
    {
        return new(in _wrap, start, start + length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Add(in CacheItem value)
    {
        using var @lock = _wrap.Location.WriteLock();
        if (@lock.RefValue.Capacity == @lock.RefValue.Count)
            return false;
        GetSpan(in @lock.RefValue)[@lock.RefValue.Count++] = value;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Insert(in int index, in CacheItem value)
    {
        using var @lock = _wrap.Location.WriteLock();
        if (@lock.RefValue.Capacity == @lock.RefValue.Count)
            return false;
        var main = GetSpan(in @lock.RefValue);
        main[index..].CopyTo(main[(index + 1)..]);
        main[index] = value;
        @lock.RefValue.Count++;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Remove(in int index)
    {
        using var @lock = _wrap.Location.WriteLock();
        var main = GetSpan(in @lock.RefValue);
        main[(index + 1)..].CopyTo(main[index..]);
        @lock.RefValue.Count--;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefEnumerator<CacheBlock, CacheItem> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    RefEnumeratorToRefReadOnlyEnumerator<RefEnumerator<CacheBlock, CacheItem>, CacheItem> IRefReadOnlyEnumerable<
            CacheBlock, CacheItem,
            RefEnumeratorToRefReadOnlyEnumerator<RefEnumerator<CacheBlock, CacheItem>, CacheItem>>.
        GetEnumerator() =>
        new(GetEnumerator());
}