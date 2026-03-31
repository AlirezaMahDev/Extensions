namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellLink<TSelf> : ICellBase<TSelf>
    where TSelf : unmanaged, ICellLink<TSelf>
{
    static abstract ComparisonWrap<ScopedComparisonChain<TSelf>, TSelf> NextComparison();
}