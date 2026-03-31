namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Cache(DataOffset offset)
    : IScopedRefReadOnlyEquatable<Cache>, IScopedInEqualityOperators<Cache, Cache, bool>
{
    public readonly DataOffset Offset = offset;
    public static readonly Cache Null = new(DataOffset.Null);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(scoped ref readonly Cache other)
    {
        return Offset == other.Offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is Cache other && Equals(in other);
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
    public static bool operator ==(scoped in Cache left, scoped in Cache right)
    {
        return left.Equals(in right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(scoped in Cache left, scoped in Cache right)
    {
        return !left.Equals(in right);
    }
}