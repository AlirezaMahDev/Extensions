namespace AlirezaMahDev.Extensions.Abstractions;

public interface IScopedRefReadOnlyComparable<T> : IComparable<T>
    where T : allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    int CompareTo(scoped ref readonly T other);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    int IComparable<T>.CompareTo(T? other)
    {
        return other is null ? -1 : CompareTo(ref other);
    }
}