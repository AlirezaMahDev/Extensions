namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparerBridge<T, TBridge, TComparer>(
    Func<T, TBridge> Func,
    TComparer Comparer) : IComparer<T>
    where TComparer : IComparer<TBridge>
    where TBridge : allows ref struct 
{
    public int Compare(T? x, T? y) => Comparer<T>.NullDown(x, y) ?? Comparer.Compare(Func(x!), Func(y!));
}