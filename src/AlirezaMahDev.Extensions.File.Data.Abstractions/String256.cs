using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[StructLayout(LayoutKind.Sequential, Size = Size)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly unsafe struct String256 : IEquatable<String256>, IString
{
    private const int Length = 256;
    private const int Size = Length * sizeof(char);
    public static String256 Empty { get; } = new();

    public static implicit operator string(String256 value)
    {
        return value.ToString();
    }

    public static implicit operator String256(string? value)
    {
        return new(value is null ? [] : value.AsSpan());
    }

    public Span<char> Span =>
        new(Unsafe.AsPointer(ref Unsafe.AsRef(in this)), Length);

    private String256(ReadOnlySpan<char> span)
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

    public bool Equals(String256 other)
    {
        return Span.SequenceEqual(other);
    }

    public override bool Equals(object? obj)
    {
        return obj is IString other && Span.SequenceEqual(other.Span);
    }

    public static bool operator ==(String256 left, String256 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(String256 left, String256 right)
    {
        return !left.Equals(right);
    }
}