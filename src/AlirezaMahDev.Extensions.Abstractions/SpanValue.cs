using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct SpanValue<T>
    where T : struct
{
    private readonly Span<T> _span;

    public SpanValue(ref T value)
    {
        _span = MemoryMarshal.CreateSpan(ref value, 1);
    }

    public SpanValue(Span<T> span)
    {
        _span = span[..1];
    }

    public ref T Value => ref MemoryMarshal.GetReference(_span);
    public static implicit operator SpanValue<T>(in T value) => new(ref Unsafe.AsRef(in value));
    public static implicit operator Span<T>(SpanValue<T> spanValue) => spanValue._span;
    public static implicit operator ReadOnlySpan<T>(SpanValue<T> spanValue) => spanValue._span;
    public static implicit operator ReadOnlySpanValue<T>(SpanValue<T> spanValue) => new(spanValue);
}