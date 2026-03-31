namespace AlirezaMahDev.Extensions.Abstractions;

public readonly struct ComparisonCollectionChain<T>(
    IEnumerable<ScopedRefReadOnlyComparison<T>> enumerable,
    ScopedRefReadOnlyComparison<T> comparison,
    ScopedRefReadOnlyComparison<T> currentComparison,
    IComparisonChain<T>? previousComparisonChain)
    : IComparisonCollection<T>, IComparisonChain<T>
{
    public IEnumerable<ScopedRefReadOnlyComparison<T>> Enumerable { get; init; } = enumerable;
    public ScopedRefReadOnlyComparison<T> Comparison { get; init; } = comparison;
    public ScopedRefReadOnlyComparison<T> CurrentComparison { get; init; } = currentComparison;
    public IComparisonChain<T>? PreviousComparisonChain { get; init; } = previousComparisonChain;
}