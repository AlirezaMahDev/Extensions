using System.Buffers;

using AlirezaMahDev.Extensions.Abstractions;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[MustDisposeResource]
public sealed class ThinkResult<TData, TLink> : IDisposable
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    private readonly ReaderWriterLockSlim _lock = new();


    private readonly MemoryList<Think<TData, TLink>> _memoryList = [];
    public Memory<Think<TData, TLink>> Thinks => _memoryList.Memory;

    public Memory<Think<TData, TLink>> GetBestThinks(int depth)
    {
        using var scoreSortComparer = Thinks.AsScoreSort();
        return Thinks[..scoreSortComparer
            .BestScoreSort(depth, NerveHelper<TData, TLink>.ThinkComparisons)];
    }

    public bool Add(Think<TData, TLink> think, int depth)
    {
        _lock.EnterWriteLock();
        try
        {
            if (CanAddCore(think, depth))
            {
                _memoryList.Add(think);
                return true;
            }

            return false;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool CanAdd(Think<TData, TLink> think, int depth)
    {
        _lock.EnterReadLock();
        try
        {
            return CanAddCore(think, depth);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    private bool CanAddCore(Think<TData, TLink> think, int depth)
    {
        if (_memoryList.Count <= Math.Max(1, depth))
        {
            return true;
        }

        var cloneLength = _memoryList.Count + 1;
        using var memoryOwner = MemoryPool<Think<TData, TLink>>.Shared.Rent(cloneLength);
        var memory = memoryOwner.Memory[..cloneLength];
        _memoryList.Memory.CopyTo(memory);

        Span<Think<TData, TLink>> span = memory.Span;
        span[^1] = think;
        using var memoryWrapComparer = memory.AsScoreSort();
        memoryWrapComparer.ScoreSort(NerveHelper<TData, TLink>.ThinkComparisons);
        return span[^1] != think;
    }

    public void Dispose()
    {
        _lock.Dispose();
        _memoryList.Dispose();
    }
}