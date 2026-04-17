namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[MustDisposeResource]
public sealed class ThinkResult<TData, TLink>(int depth, ScopedRefReadOnlyFunc<Think<TData, TLink>, bool> check) : IDisposable
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly ScopedRefReadOnlyComparisonToScopedRefReadOnlyComparer<Think<TData, TLink>> _scopedRefReadOnlyComparisonToScopedRefReadOnlyComparer =
        new(NerveHelper<TData, TLink>.ThinkComparisons.Comparison);
    private readonly MemoryList<Think<TData, TLink>> _memoryList = [];

    public Memory<Think<TData, TLink>> Thinks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => _memoryList.Memory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Memory<Think<TData, TLink>> GetBestThinks()
    {
        _lock.EnterWriteLock();
        try
        {
            return Thinks.ToArray();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryAdd(Think<TData, TLink> think)
    {
        _lock.EnterUpgradeableReadLock();
        try
        {
            var canAddCore = CanAddCore(think);
            if (canAddCore > 0)
            {
                return false;
            }

            _lock.EnterWriteLock();
            try
            {
                canAddCore = CanAddCore(think);
                if (canAddCore > 0)
                {
                    return false;
                }

                if (!check(ref think))
                {
                    return false;
                }

                _memoryList.Add(think);
                _memoryList.Memory.Span.Sort(_scopedRefReadOnlyComparisonToScopedRefReadOnlyComparer);
                if (canAddCore < 0)
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
            return CanAddCore(think) is null or <= 0;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private int? CanAddCore(Think<TData, TLink> think)
    {
        return _memoryList.Count <= Math.Max(1, depth)
            ? null
            : _scopedRefReadOnlyComparisonToScopedRefReadOnlyComparer.Compare(ref think, ref _memoryList.Memory.Span[^1]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _lock.Dispose();
        _memoryList.Dispose();
    }
}