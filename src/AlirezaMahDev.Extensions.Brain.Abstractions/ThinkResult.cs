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
            if (CanAddCore(think))
            {
                _lock.EnterWriteLock();
                try
                {
                    _memoryList.Add(think);
                    _memoryList.Memory.Span.Sort(NerveHelper<TData, TLink>.ThinkComparisons.Comparison);
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
            return CanAddCore(think);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    private bool CanAddCore(Think<TData, TLink> think) =>
        _memoryList.Count <= Math.Max(1, depth) ||
            NerveHelper<TData, TLink>.ThinkComparisons.Comparison(think, _memoryList.Memory.Span[^1]) <= 0;

    public void Dispose()
    {
        _lock.Dispose();
        _memoryList.Dispose();
    }
}