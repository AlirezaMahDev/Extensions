namespace AlirezaMahDev.Extensions.Abstractions;

public interface IComparisonCollection<T>
        where T : allows ref struct
{
    IEnumerable<Comparison<T>> Enumerable { get; set; }
}