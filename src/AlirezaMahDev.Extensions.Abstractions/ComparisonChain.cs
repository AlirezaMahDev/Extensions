namespace AlirezaMahDev.Extensions.Abstractions;

public record struct ComparisonChain<T>(Comparison<T> Comparison) : IComparisonChain<T>;