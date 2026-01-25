using System.Buffers;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
public class ScoreSortComparer<T> : IDisposable, IComparer<T>
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
            comparisons) =>
        Memory[..BestScoreSort(depth, comparisons)];

    public int BestScoreSort(int depth,
        ComparisonBuilder<ComparisonCollectionChain<ScoreSortItem<T>>, ScoreSortItem<T>>
            comparisons)
    {
        var count = WrapMemory.Span.BestScoreSort(depth, comparisons);
        Apply();
        return count;
    }

    private void Apply()
    {
        var wrapMemorySpan = WrapMemory.Span;
        var memorySpan = Memory.Span;
        for (int i = 0; i < wrapMemorySpan.Length; i++)
        {
            memorySpan[i] = wrapMemorySpan[i].Value;
        }
    }

    public void Dispose()
    {
        _memoryOwner.Dispose();
        GC.SuppressFinalize(this);
    }

    public int Compare(T? x, T? y) =>
        Comparer<T>.NullDown(x, y) ??
        ScoreSortHelper<ScoreSortItem<T>>.Comparison(Find(x!), Find(y!));

    private ScoreSortItem<T> Find(T t) =>
        WrapMemory.Span.First((in x) => x.Value.Equals(t));
}