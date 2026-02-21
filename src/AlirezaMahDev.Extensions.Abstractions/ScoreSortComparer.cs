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

        var srcSpan = Memory.Span;
        var distSpan = _memoryOwner.Memory.Span;
        for (var i = 0; i < srcSpan.Length; i++)
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
            comparisons) =>
        Memory[..BestScoreSort(depth, comparisons)];

    public Memory<T> TakeBestScoreSortClone(int depth,
        ComparisonBuilder<ComparisonCollectionChain<ScoreSortItem<T>>, ScoreSortItem<T>>
            comparisons)
    {
        var source = WrapMemory[..BestScoreSortCore(depth, comparisons)];
        var dist = new T[source.Length].AsMemory();
        var sourceSpan = source.Span;
        var distSpan = dist.Span;
        for (var i = 0; i < sourceSpan.Length; i++)
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
        var count = BestScoreSortCore(depth, comparisons);
        Apply();
        return count;
    }

    private void Apply()
    {
        var wrapMemorySpan = WrapMemory.Span;
        var memorySpan = Memory.Span;
        for (var i = 0; i < wrapMemorySpan.Length; i++)
        {
            memorySpan[i] = wrapMemorySpan[i].Value;
        }
    }

    public void Dispose()
    {
        _memoryOwner.Dispose();
    }

    public int Compare(T? x, T? y) =>
        Comparer<T>.NullDown(x, y) ??
        ScoreSortHelper<ScoreSortItem<T>>.Comparison(Find(x!), Find(y!));

    private ScoreSortItem<T> Find(T t) =>
        WrapMemory.Span.First((in x) => x.Value.Equals(t));
}