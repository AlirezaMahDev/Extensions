namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellBase<TSelf> : IScopedRefReadOnlyEquatable<TSelf>,
    IScopedInEqualityOperators<TSelf, TSelf, bool>,
    IScopedRefReadOnlyComparable<TSelf>,
    IMultiplyOperators<TSelf, TSelf, TSelf>,
    IDivisionOperators<TSelf, TSelf, TSelf>,
    IAdditionOperators<TSelf, TSelf, TSelf>,
    ISubtractionOperators<TSelf, TSelf, TSelf>
    where TSelf : unmanaged, ICellBase<TSelf>
{
    TSelf Abs();
    TSelf Normalize();
    TSelf UnNormalize();
    static abstract ComparisonWrap<ScopedComparisonChain<TSelf>, TSelf> SleepComparison();

    static abstract TSelf ThinkDifference(scoped ref readonly TSelf previous,
        scoped ref readonly TSelf real,
        scoped ref readonly TSelf predict);

    static abstract ComparisonWrap<ScopedComparisonChain<TSelf>, TSelf> ThinkComparison();
}