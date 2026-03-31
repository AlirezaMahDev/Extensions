namespace AlirezaMahDev.Extensions.Abstractions;

public interface IComparisonChain<T>
    where T : allows ref struct
{
    ScopedRefReadOnlyComparison<T> Comparison { get; init; }
    ScopedRefReadOnlyComparison<T> CurrentComparison { get; init; }
    IComparisonChain<T>? PreviousComparisonChain { get; init; }
}