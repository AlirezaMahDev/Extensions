namespace AlirezaMahDev.Extensions.Abstractions;

public interface IInComparer<T> : IComparer<T>
    where T : allows ref struct
{
    int Compare(in T? x, in T? y);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    int IComparer<T>.Compare(T? x, T? y) =>
        Compare(in x, in y);
}