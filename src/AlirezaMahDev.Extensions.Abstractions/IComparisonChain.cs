namespace AlirezaMahDev.Extensions.Abstractions;

public interface IComparisonChain<T>
    where T : allows ref struct
{
    Comparison<T> Comparison { get; set; }
    Comparison<T> CurrentComparison { get; set; }
    IComparisonChain<T>? PreviousComparisonChain { get; set; }
}