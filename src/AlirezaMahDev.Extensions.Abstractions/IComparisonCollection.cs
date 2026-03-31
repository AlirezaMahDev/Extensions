namespace AlirezaMahDev.Extensions.Abstractions;

public interface IComparisonCollection<T>
    where T : allows ref struct
{
    IEnumerable<ScopedRefReadOnlyComparison<T>> Enumerable { get; init; }
}