namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Size = Size)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct String256 : IScopedRefReadOnlyEquatable<String256>, IString<String256>
{
    private const int Length = 256;
    private const int Size = Length * sizeof(char);

    public static readonly String256 Empty = new();

    public ReadOnlySpan<char> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => MemoryMarshal.CreateReadOnlySpan(
            ref Unsafe.As<String256, char>(
                ref Unsafe.AsRef(in this)),
            Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private String256(ReadOnlySpan<char> value)
    {
        if ((uint)value.Length > Length)
        {
            ThrowHelper.ThrowArgumentException(
                $"{value.Length} > {Length}.",
                nameof(value));
        }

        ref var dest = ref Unsafe.As<String256, char>(ref this);
        value.CopyTo(MemoryMarshal.CreateSpan(ref dest, Length));

        if (value.Length < Length)
        {
            MemoryMarshal.CreateSpan(
                    ref Unsafe.Add(ref dest, value.Length),
                    Length - value.Length)
                .Clear();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string(String256 value)
    {
        return value.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator String256(string? value)
    {
        return value is null or { Length: 0 }
            ? Empty
            : new(value.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override string ToString()
    {
        return new(Span.TrimEnd('\0'));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(scoped ref readonly String256 other)
    {
        return MemoryMarshal.AsBytes(Span)
            .SequenceEqual(MemoryMarshal.AsBytes(other.Span));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is String256 other && Equals(ref other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return (int)XxHash3.HashToUInt64(MemoryMarshal.AsBytes(Span));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(String256 left, String256 right)
    {
        return left.Equals(ref right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(String256 left, String256 right)
    {
        return !left.Equals(ref right);
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}