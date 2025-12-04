using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = Size)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly unsafe struct String128 : IEquatable<String128>, IString
{
    private const int Length = 128;
    private const int Size = Length * sizeof(char);
    public static String128 Empty { get; } = new();

    public static implicit operator string(String128 value)
    {
        return value.ToString();
    }

    public static implicit operator String128(string? value)
    {
        return new(value is null ? [] : value.AsSpan());
    }

    public Span<char> Span =>
        new(Unsafe.AsPointer(ref Unsafe.AsRef(in this)), Length);

    private String128(ReadOnlySpan<char> span)
    {
        if (span.Length > Length)
        {
            throw new ArgumentException($"{span.Length} > {Length}.", nameof(span));
        }

        Span.Clear();
        span.CopyTo(Span);
    }

    public override string ToString()
    {
        return new string(Span).Trim();
    }

    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.AddBytes(MemoryMarshal.AsBytes(Span));
        return hashCode.ToHashCode();
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }

    public bool Equals(String128 other)
    {
        return Span.SequenceEqual(other);
    }

    public override bool Equals(object? obj)
    {
        return obj is IString other && Span.SequenceEqual(other.Span);
    }

    public static bool operator ==(String128 left, String128 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(String128 left, String128 right)
    {
        return !left.Equals(right);
    }
}