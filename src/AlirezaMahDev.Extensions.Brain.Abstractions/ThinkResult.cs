namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[MustDisposeResource]
public sealed class ThinkResult<TData, TLink>(int depth) : IDisposable
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    private readonly ReaderWriterLockSlim _lock = new();

    private readonly MemoryList<Think<TData, TLink>> _memoryList = [];

    public Memory<Think<TData, TLink>> Thinks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => _memoryList.Memory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Memory<Think<TData, TLink>> GetBestThinks()
    {
        ScopedRefReadOnlyComparisonToScopedRefReadOnlyComparer<Think<TData, TLink>> scopedRefReadOnlyComparisonToScopedRefReadOnlyComparer =
            new(NerveHelper<TData, TLink>
                .ThinkComparisons.Comparison);
        Thinks.Span.Sort(scopedRefReadOnlyComparisonToScopedRefReadOnlyComparer);
        return Thinks.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Add(Think<TData, TLink> think)
    {
        _lock.EnterUpgradeableReadLock();
        try
        {
            var canAddCore = CanAddCore(think);
            if (canAddCore != false)
            {
                _lock.EnterWriteLock();
                try
                {
                    _memoryList.Add(think);
                    ScopedRefReadOnlyComparisonToScopedRefReadOnlyComparer<Think<TData, TLink>>
                        scopedRefReadOnlyComparisonToScopedRefReadOnlyComparer =
                            new(NerveHelper<TData, TLink>
                                .ThinkComparisons.Comparison);
                    _memoryList.Memory.Span.Sort(scopedRefReadOnlyComparisonToScopedRefReadOnlyComparer);
                    if (canAddCore != null)
                    {
                        _memoryList.RemoveAt(_memoryList.Count - 1);
                    }

                    return true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }

            return false;
        }
        finally
        {
            _lock.ExitUpgradeableReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool CanAdd(Think<TData, TLink> think)
    {
        _lock.EnterReadLock();
        try
        {
            return CanAddCore(think) != false;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool? CanAddCore(Think<TData, TLink> think)
    {
        if (_memoryList.Count <= Math.Max(1, depth))
        {
            return null;
        }

        var comparison =
            NerveHelper<TData, TLink>.ThinkComparisons.Comparison(ref think, ref _memoryList.Memory.Span[^1]);
        return comparison == 0 ? null : comparison < 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _lock.Dispose();
        _memoryList.Dispose();
    }
}