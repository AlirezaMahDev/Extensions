namespace AlirezaMahDev.Extensions.Abstractions;

public interface IComparisonChain<T>
{
    Comparison<T> Comparison { get; set; }
}