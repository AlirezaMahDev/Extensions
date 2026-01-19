using System.Buffers;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

public readonly struct ScoreSortMemoryWrap<T> : IDisposable, IComparer<T>
    where T : notnull
{
    public Memory<T> Memory { get; }
    public Memory<ScoreSortMemoryWrapItem<T>> WrapMemory { get; }
    private readonly IMemoryOwner<ScoreSortMemoryWrapItem<T>> _memoryOwner;

    public ScoreSortMemoryWrap(Memory<T> memory)
    {
        Memory = memory;
        _memoryOwner = MemoryPool<ScoreSortMemoryWrapItem<T>>.Shared.Rent(memory.Length);
        WrapMemory = _memoryOwner.Memory[..memory.Length];

        var srcSpan = Memory.Span;
        var distSpan = _memoryOwner.Memory.Span;
        for (int i = 0; i < srcSpan.Length; i++)
        {
            distSpan[i] = new(srcSpan[i]);
        }
    }


    public void Sort(params ReadOnlySpan<Comparison<T>> comparisons)
    {
        using var convertComparison = ConvertComparison(comparisons);
        WrapMemory.Span.ScoreSort(convertComparison.Value.Span);
        Apply();
    }

    public Memory<T> TakeBestSort(int depth, params ReadOnlySpan<Comparison<T>> comparisons) =>
        Memory[..BestSort(depth, comparisons)];

    public int BestSort(int depth, params ReadOnlySpan<Comparison<T>> comparisons)
    {
        using var convertComparison = ConvertComparison(comparisons);
        var count = WrapMemory.Span.BestScoreSort(depth, convertComparison.Value.Span);
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

    [MustDisposeResource]
    private static DisposableValue<Memory<Comparison<ScoreSortMemoryWrapItem<T>>>> ConvertComparison(
        params ReadOnlySpan<Comparison<T>> comparisons)
    {
        var memoryPool = MemoryPool<Comparison<ScoreSortMemoryWrapItem<T>>>
            .Shared.Rent(comparisons.Length);
        var memory = memoryPool.Memory[..comparisons.Length];
        var span = memory.Span;
        for (int i = 0; i < span.Length; i++)
        {
            var comparison = comparisons[i];
            span[i] = (x, y) =>
                comparison(x.Value, y.Value);
        }

        return new(memory, memoryPool);
    }


    public void Dispose()
    {
        _memoryOwner.Dispose();
    }

    public int Compare(T? x, T? y) =>
        x is null
            ? 1
            : y is null
                ? 1
                : ScoreSortItemComparer<ScoreSortMemoryWrapItem<T>>.Default.Compare(Find(x), Find(y));

    private ScoreSortMemoryWrapItem<T> Find(T t) =>
        WrapMemory.Span.First((in x) => x.Value.Equals(t));
}