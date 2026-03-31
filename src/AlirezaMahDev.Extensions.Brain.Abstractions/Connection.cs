namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Connection(DataOffset offset)
    : IScopedRefReadOnlyEquatable<Connection>, IScopedInEqualityOperators<Connection, Connection, bool>
{
    public readonly DataOffset Offset = offset;
    public static readonly Connection Null = new(DataOffset.Null);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(scoped ref readonly Connection other)
    {
        return Offset == other.Offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is Connection other && Equals(in other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return XxHash3.Combine(in Offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override string ToString()
    {
        return $"F:{Offset}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(scoped in Connection left, scoped in Connection right)
    {
        return left.Equals(in right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(scoped in Connection left, scoped in Connection right)
    {
        return !left.Equals(in right);
    }
}
