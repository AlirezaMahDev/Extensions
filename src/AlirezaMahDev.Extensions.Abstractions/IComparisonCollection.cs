namespace AlirezaMahDev.Extensions.Abstractions;

public interface IComparisonCollection<T>
{
    IEnumerable<Comparison<T>> Enumerable { get; set; }
}