using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class UnmanagedExtensions
{
    extension<T>(T value)
        where T : unmanaged
    {
        public Span<byte> AsSpan() =>
            MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1));
        public ReadOnlySpan<byte> AsReadOnlySpan() =>
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1));
    }
}
