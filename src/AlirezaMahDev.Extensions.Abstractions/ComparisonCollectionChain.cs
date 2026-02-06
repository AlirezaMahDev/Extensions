namespace AlirezaMahDev.Extensions.Abstractions;

public record struct ComparisonCollectionChain<T>(IEnumerable<Comparison<T>> Enumerable, Comparison<T> Comparison, Comparison<T> CurrentComparison, IComparisonChain<T>? PreviousComparisonChain)
    : IComparisonCollection<T>, IComparisonChain<T>;