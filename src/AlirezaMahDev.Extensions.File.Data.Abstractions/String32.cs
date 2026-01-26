using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[StructLayout(LayoutKind.Sequential, Size = Size)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly unsafe struct String32 : IEquatable<String32>, IString
{
    private const int Length = 32;
    private const int Size = Length * sizeof(char);
    public static String32 Empty { get; } = new();

    public static implicit operator string(String32 value)
    {
        return value.ToString();
    }

    public static implicit operator String32(string? value)
    {
        return new(value is null ? [] : value.AsSpan());
    }

    public Span<char> Span =>
        new(Unsafe.AsPointer(ref Unsafe.AsRef(in this)), Length);

    private String32(ReadOnlySpan<char> span)
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

    public bool Equals(String32 other)
    {
        return Span.SequenceEqual(other);
    }

    public override bool Equals(object? obj)
    {
        return obj is IString other && Span.SequenceEqual(other.Span);
    }

    public static bool operator ==(String32 left, String32 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(String32 left, String32 right)
    {
        return !left.Equals(right);
    }
}