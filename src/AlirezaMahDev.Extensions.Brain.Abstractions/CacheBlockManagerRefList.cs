namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Auto)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct CacheBlockManagerRefList(ref readonly DataWrap<CacheValue, DataEmptyWrap> wrap)
    : IRefList<CacheBlockManagerRefList, CacheItem, RefEnumerator<CacheBlockManagerRefList, CacheItem>>
{
    private readonly ref readonly DataWrap<CacheValue, DataEmptyWrap> _wrap = ref wrap;
    private readonly Index _start = 0;
    private readonly Index _end = ^0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private CacheBlockManagerRefList(ref readonly DataWrap<CacheValue, DataEmptyWrap> wrap,
        Index start,
        Index end) :
        this(in wrap)
    {
        _start = start;
        _end = end;
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => _wrap.Location.ReadLock((scoped ref readonly value) => value.Count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private SpanWrap<CacheBlockValue> GetSpanWrap(scoped ref readonly CacheBlockValue value)
    {
        throw new NotImplementedException();
    }

    public ref CacheItem this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            throw new NotImplementedException();

        }
    }

    ref readonly CacheItem
        IRefReadOnlyBlock<CacheBlockManagerRefList, CacheItem, RefEnumeratorToRefReadOnlyEnumerator<
            RefEnumerator<CacheBlockManagerRefList, CacheItem>, CacheItem>>.this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref this[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public CacheBlockManagerRefList Slice(int start, int length) =>
        new(in _wrap, start, start + length);

    public bool Add(in CacheItem value)
    {
        throw new NotImplementedException();
    }

    public bool Insert(in int index, in CacheItem value)
    {
        throw new NotImplementedException();
    }

    public bool Remove(in int index)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefEnumerator<CacheBlockManagerRefList, CacheItem> GetEnumerator() =>
        new(this);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    RefEnumeratorToRefReadOnlyEnumerator<RefEnumerator<CacheBlockManagerRefList, CacheItem>, CacheItem>
        IRefReadOnlyEnumerable<CacheBlockManagerRefList, CacheItem, RefEnumeratorToRefReadOnlyEnumerator<
            RefEnumerator<CacheBlockManagerRefList, CacheItem>, CacheItem>>.GetEnumerator() =>
        new(GetEnumerator());
}