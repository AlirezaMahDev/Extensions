namespace AlirezaMahDev.Extensions.Abstractions;

public interface IScopedRefReadOnlyEquatable<T> : IEquatable<T>
    where T : allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool Equals(scoped ref readonly T other);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool IEquatable<T>.Equals(T? other)
    {
        return other is not null && Equals(ref other);
    }
}