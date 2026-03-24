namespace AlirezaMahDev.Extensions.Abstractions;

public interface IInComparable<T> : IComparable<T>
    where T : allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    int CompareTo(in T? other);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    int IComparable<T>.CompareTo(T? other) =>
        CompareTo(in other);
}