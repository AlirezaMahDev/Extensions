using AlirezaMahDev.Extensions.Abstractions;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[MustDisposeResource]
public sealed class ThinkResult<TData, TLink>(int depth) : IDisposable
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    private readonly ReaderWriterLockSlim _lock = new();

    private readonly MemoryList<Think<TData, TLink>> _memoryList = [];
    public Memory<Think<TData, TLink>> Thinks => _memoryList.Memory;

    public Memory<Think<TData, TLink>> GetBestThinks()
    {
        Thinks.Span.Sort(NerveHelper<TData, TLink>.ThinkComparisons.Comparison);
        return Thinks.ToArray();
    }

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
                    _memoryList.Memory.Span.Sort(NerveHelper<TData, TLink>.ThinkComparisons.Comparison);
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

    private bool? CanAddCore(Think<TData, TLink> think)
    {
        if (_memoryList.Count <= Math.Max(1, depth))
        {
            return null;
        }

        var comparison = NerveHelper<TData, TLink>.ThinkComparisons.Comparison(think, _memoryList.Memory.Span[^1]);
        return comparison == 0 ? null : comparison < 0;
    }

    public void Dispose()
    {
        _lock.Dispose();
        _memoryList.Dispose();
    }
}