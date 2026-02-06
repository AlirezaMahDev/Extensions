namespace AlirezaMahDev.Extensions.Abstractions;

public interface IComparisonChain<T>
{
    Comparison<T> Comparison { get; set; }
    Comparison<T> CurrentComparison { get; set; }
    IComparisonChain<T>? PreviousComparisonChain { get; set; }
}