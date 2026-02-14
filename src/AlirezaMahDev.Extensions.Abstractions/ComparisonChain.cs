namespace AlirezaMahDev.Extensions.Abstractions;

public record struct ComparisonChain<T>(
    Comparison<T> Comparison,
    Comparison<T> CurrentComparison,
    IComparisonChain<T>? PreviousComparisonChain)
    : IComparisonChain<T>, IComparer<T>
    where T : allows ref struct
{
    public readonly int Compare(T? x, T? y) => ComparerHelper<T>.NullDown(x, y) ?? Comparison(x!, y!);
}