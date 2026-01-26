using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct NerveCacheKey(ReadOnlySpan<byte> bytes)
{
    public UInt128 Hash { get; } = XxHash128.HashToUInt128(bytes);

    public static NerveCacheKey Create<T1>(in T1 t1)
        where T1 : unmanaged =>
        new(AsReadOnlySpan(in t1));

    public static NerveCacheKey Create<T1, T2>(in T1 t1, in T2 t2)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        var s1 = AsReadOnlySpan(in t1);
        var s2 = AsReadOnlySpan(in t2);

        int length = s1.Length + s2.Length;

        Span<byte> destination = stackalloc byte[length];

        int offset = 0;
        s1.CopyTo(destination[offset..]);
        offset += s1.Length;
        s2.CopyTo(destination[offset..]);

        return new(destination);
    }

    public static NerveCacheKey Create<T1, T2, T3>(in T1 t1, in T2 t2, in T3 t3)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        var s1 = AsReadOnlySpan(in t1);
        var s2 = AsReadOnlySpan(in t2);
        var s3 = AsReadOnlySpan(in t3);

        int length = s1.Length + s2.Length + s3.Length;

        Span<byte> destination = stackalloc byte[length];

        int offset = 0;
        s1.CopyTo(destination[offset..]);
        offset += s1.Length;
        s2.CopyTo(destination[offset..]);
        offset += s2.Length;
        s3.CopyTo(destination[offset..]);

        return new(destination);
    }

    public static NerveCacheKey Create<T1, T2, T3, T4>(in T1 t1, in T2 t2, in T3 t3, in T4 t4)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        var s1 = AsReadOnlySpan(in t1);
        var s2 = AsReadOnlySpan(in t2);
        var s3 = AsReadOnlySpan(in t3);
        var s4 = AsReadOnlySpan(in t4);

        int length = s1.Length + s2.Length + s3.Length + s4.Length;

        Span<byte> destination = stackalloc byte[length];

        int offset = 0;
        s1.CopyTo(destination[offset..]);
        offset += s1.Length;
        s2.CopyTo(destination[offset..]);
        offset += s2.Length;
        s3.CopyTo(destination[offset..]);
        offset += s3.Length;
        s4.CopyTo(destination[offset..]);

        return new(destination);
    }

    public static ReadOnlySpan<byte> AsReadOnlySpan<T>(in T value) => MemoryMarshal.CreateReadOnlySpan(
        ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in value)),
        Unsafe.SizeOf<T>());
}