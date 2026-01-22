namespace AlirezaMahDev.Extensions.Abstractions;

public record struct ComparisonCollectionChain<T>(IEnumerable<Comparison<T>> Enumerable, Comparison<T> Comparison)
    : IComparisonCollection<T>, IComparisonChain<T>;