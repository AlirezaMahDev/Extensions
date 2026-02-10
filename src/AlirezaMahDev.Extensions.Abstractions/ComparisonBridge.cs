namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparisonBridge<T, TBridge>(
    Func<T, TBridge> Func,
    Comparison<TBridge> Comparison) : IComparer<T>
{
    public int Compare(T? x, T? y) => Comparer<T>.NullDown(x, y) ?? Comparison(Func(x!), Func(y!));
}