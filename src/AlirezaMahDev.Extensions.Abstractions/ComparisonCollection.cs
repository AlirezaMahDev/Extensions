namespace AlirezaMahDev.Extensions.Abstractions;

public record struct ComparisonCollection<T>(IEnumerable<Comparison<T>> Enumerable) : IComparisonCollection<T>
{
    public static ComparisonCollection<T> Create() => new([]);
}