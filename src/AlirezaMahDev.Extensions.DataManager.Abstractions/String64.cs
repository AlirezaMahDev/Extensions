namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Size = Size)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct String64 : IInEquatable<String64>, IString<String64>
{
    private const int Length = 64;
    private const int Size = Length * sizeof(char);

    public static readonly String64 Empty = new();

    public ReadOnlySpan<char> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => MemoryMarshal.CreateReadOnlySpan(
            ref Unsafe.As<String64, char>(
                ref Unsafe.AsRef(in this)),
            Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private String64(ReadOnlySpan<char> value)
    {
        if ((uint)value.Length > Length)
        {
            ThrowHelper.ThrowArgumentException(
                $"{value.Length} > {Length}.",
                nameof(value));
        }

        ref var dest = ref Unsafe.As<String64, char>(ref this);
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
    public static implicit operator string(String64 value) => value.ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator String64(string? value)
        => value is null or { Length: 0 }
            ? Empty
            : new(value.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override string ToString() => new(Span.TrimEnd('\0'));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(in String64 other)
        => MemoryMarshal.AsBytes(Span)
            .SequenceEqual(MemoryMarshal.AsBytes(other.Span));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
        => obj is String64 other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
        => (int)XxHash3.HashToUInt64(MemoryMarshal.AsBytes(Span));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(String64 left, String64 right)
        => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(String64 left, String64 right)
        => !left.Equals(right);

    private string GetDebuggerDisplay() => ToString();
}