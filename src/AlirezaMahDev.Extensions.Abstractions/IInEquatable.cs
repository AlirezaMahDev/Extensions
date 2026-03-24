namespace AlirezaMahDev.Extensions.Abstractions;

public interface IInEquatable<T> : IEquatable<T>
    where T : allows ref struct
{
    bool Equals(in T? other);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool IEquatable<T>.Equals(T? other) => Equals(in other);
}