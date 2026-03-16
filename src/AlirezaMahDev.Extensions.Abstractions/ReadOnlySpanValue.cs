using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct ReadOnlySpanValue<T>
    where T : struct
{
    private readonly ReadOnlySpan<T> _readOnlySpan;

    public ReadOnlySpanValue(in T value)
    {
        _readOnlySpan = MemoryMarshal.CreateReadOnlySpan(in value, 1);
    }

    public ReadOnlySpanValue(ReadOnlySpan<T> readOnlySpan)
    {
        _readOnlySpan = readOnlySpan[..1];
    }

    public bool HasValue => !_readOnlySpan.IsEmpty;
    public ref readonly T Value => ref MemoryMarshal.GetReference(_readOnlySpan);

    public static implicit operator ReadOnlySpanValue<T>(in T value)
    {
        return new(in value);
    }

    public static implicit operator ReadOnlySpan<T>(ReadOnlySpanValue<T> readOnlySpanValue)
    {
        return readOnlySpanValue._readOnlySpan;
    }
}