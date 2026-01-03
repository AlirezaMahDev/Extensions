using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public readonly struct NerveCacheKey(params ReadOnlySpan<byte> bytes)
{
    public UInt128 Hash { get; } = XxHash128.HashToUInt128(bytes);

    public static NerveCacheKey Create<T1>(in T1 t1)
        where T1 : unmanaged =>
            new(AsReadOnlySpan(in t1));

    public static NerveCacheKey Create<T1, T2>(in T1 t1, in T2 t2)
        where T1 : unmanaged where T2 : unmanaged =>
            new([.. AsReadOnlySpan(in t1), .. AsReadOnlySpan(in t2)]);

    public static NerveCacheKey Create<T1, T2, T3>(in T1 t1, in T2 t2, in T3 t3)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged =>
            new([.. AsReadOnlySpan(in t1), .. AsReadOnlySpan(in t2), .. AsReadOnlySpan(in t3)]);

    public static NerveCacheKey Create<T1, T2, T3, T4>(in T1 t1, in T2 t2, in T3 t3, in T4 t4)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged =>
            new([.. AsReadOnlySpan(in t1), .. AsReadOnlySpan(in t2), .. AsReadOnlySpan(in t3), .. AsReadOnlySpan(in t4)]);

    public static NerveCacheKey Create<T1, T2, T3, T4, T5>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged =>
            new([.. AsReadOnlySpan(in t1), .. AsReadOnlySpan(in t2), .. AsReadOnlySpan(in t3), .. AsReadOnlySpan(in t4), .. AsReadOnlySpan(in t5)]);

    public static NerveCacheKey Create<T1, T2, T3, T4, T5, T6>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5, in T6 t6)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged =>
            new([.. AsReadOnlySpan(in t1), .. AsReadOnlySpan(in t2), .. AsReadOnlySpan(in t3), .. AsReadOnlySpan(in t4), .. AsReadOnlySpan(in t5), .. AsReadOnlySpan(in t6)]);

    public static NerveCacheKey Create<T1, T2, T3, T4, T5, T6, T7>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5, in T6 t6, in T7 t7)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged where T7 : unmanaged =>
            new([.. AsReadOnlySpan(in t1), .. AsReadOnlySpan(in t2), .. AsReadOnlySpan(in t3), .. AsReadOnlySpan(in t4), .. AsReadOnlySpan(in t5), .. AsReadOnlySpan(in t6), .. AsReadOnlySpan(in t7)]);

    public static ReadOnlySpan<byte> AsReadOnlySpan<T>(in T value) => MemoryMarshal.CreateReadOnlySpan(
        ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in value)),
    Unsafe.SizeOf<T>());
}
