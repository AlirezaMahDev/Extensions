namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Neuron(DataOffset offset)
    : IScopedRefReadOnlyEquatable<Neuron>, IScopedInEqualityOperators<Neuron, Neuron, bool>
{
    public readonly DataOffset Offset = offset;
    public static readonly Neuron Null = new(DataOffset.Null);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(scoped ref readonly Neuron other)
    {
        return Offset == other.Offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is Neuron other && Equals(in other);
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
    public static bool operator ==(scoped in Neuron left, scoped in Neuron right)
    {
        return left.Equals(in right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(scoped in Neuron left, scoped in Neuron right)
    {
        return !left.Equals(in right);
    }
}