using System.Buffers;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
public sealed class ScoreSortComparer<T> : IDisposable, IComparer<T>
    where T : notnull
{
    public Memory<T> Memory { get; }
    public Memory<ScoreSortItem<T>> WrapMemory { get; }
    private readonly IMemoryOwner<ScoreSortItem<T>> _memoryOwner;

    public ScoreSortComparer(Memory<T> memory)
    {
        Memory = memory;
        _memoryOwner = MemoryPool<ScoreSortItem<T>>.Shared.Rent(memory.Length);
        WrapMemory = _memoryOwner.Memory[..memory.Length];

        Span<T> srcSpan = Memory.Span;
        Span<ScoreSortItem<T>> distSpan = _memoryOwner.Memory.Span;
        for (int i = 0; i < srcSpan.Length; i++)
        {
            distSpan[i] = new(srcSpan[i]);
        }
    }

    public void ScoreSort(
        ComparisonBuilder<ComparisonCollectionChain<ScoreSortItem<T>>, ScoreSortItem<T>> builder)
    {
        WrapMemory.Span.ScoreSort(builder);
        Apply();
    }

    public Memory<T> TakeBestScoreSort(int depth,
        ComparisonBuilder<ComparisonCollectionChain<ScoreSortItem<T>>, ScoreSortItem<T>>
            comparisons)
    {
        return Memory[..BestScoreSort(depth, comparisons)];
    }

    public Memory<T> TakeBestScoreSortClone(int depth,
        ComparisonBuilder<ComparisonCollectionChain<ScoreSortItem<T>>, ScoreSortItem<T>>
            comparisons)
    {
        Memory<ScoreSortItem<T>> source = WrapMemory[..BestScoreSortCore(depth, comparisons)];
        Memory<T> dist = new T[source.Length].AsMemory();
        Span<ScoreSortItem<T>> sourceSpan = source.Span;
        Span<T> distSpan = dist.Span;
        for (int i = 0; i < sourceSpan.Length; i++)
        {
            distSpan[i] = sourceSpan[i].Value;
        }

        return dist;
    }

    public int BestScoreSortCore(int depth,
        ComparisonBuilder<ComparisonCollectionChain<ScoreSortItem<T>>, ScoreSortItem<T>>
            comparisons)
    {
        return WrapMemory.Span.BestScoreSort(depth, comparisons);
    }

    public int BestScoreSort(int depth,
        ComparisonBuilder<ComparisonCollectionChain<ScoreSortItem<T>>, ScoreSortItem<T>>
            comparisons)
    {
        int count = BestScoreSortCore(depth, comparisons);
        Apply();
        return count;
    }

    private void Apply()
    {
        Span<ScoreSortItem<T>> wrapMemorySpan = WrapMemory.Span;
        Span<T> memorySpan = Memory.Span;
        for (int i = 0; i < wrapMemorySpan.Length; i++)
        {
            memorySpan[i] = wrapMemorySpan[i].Value;
        }
    }

    public void Dispose()
    {
        _memoryOwner.Dispose();
    }

    public int Compare(T? x, T? y)
    {
        return Comparer<T>.NullDown(x, y) ??
               ScoreSortHelper<ScoreSortItem<T>>.Comparison(Find(x!), Find(y!));
    }

    private ScoreSortItem<T> Find(T t)
    {
        return WrapMemory.Span.First((in x) => x.Value.Equals(t));
    }
}