namespace AlirezaMahDev.Extensions.Abstractions;

public record struct ComparisonChain<T>(Comparison<T> Comparison, Comparison<T> CurrentComparison, IComparisonChain<T>? PreviousComparisonChain)
    : IComparisonChain<T>, IComparer<T>
{
    public readonly int Compare(T? x, T? y) => Comparer<T>.NullDown(x, y) ?? Comparison(x!, y!);
}