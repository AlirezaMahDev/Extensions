using System.IO.Hashing;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

readonly struct NerveCacheKey
{
    public UInt128 Hash { get; }

    public NerveCacheKey(params ReadOnlySpan<byte> bytes)
    {
        Hash = XxHash128.HashToUInt128(bytes);
    }

    public static NerveCacheKey Create<T1>(in T1 t1)
        where T1 : unmanaged =>
            new(t1.AsReadOnlySpan());
    public static NerveCacheKey Create<T1, T2>(in T1 t1, in T2 t2)
        where T1 : unmanaged where T2 : unmanaged =>
            new([.. t1.AsReadOnlySpan(), .. t2.AsReadOnlySpan()]);
    public static NerveCacheKey Create<T1, T2, T3>(in T1 t1, in T2 t2, in T3 t3)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged =>
            new([.. t1.AsReadOnlySpan(), .. t2.AsReadOnlySpan(), .. t3.AsReadOnlySpan()]);
    public static NerveCacheKey Create<T1, T2, T3, T4>(in T1 t1, in T2 t2, in T3 t3, in T4 t4)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged =>
            new([.. t1.AsReadOnlySpan(), .. t2.AsReadOnlySpan(), .. t3.AsReadOnlySpan(), .. t4.AsReadOnlySpan()]);
    public static NerveCacheKey Create<T1, T2, T3, T4, T5>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged =>
            new([.. t1.AsReadOnlySpan(), .. t2.AsReadOnlySpan(), .. t3.AsReadOnlySpan(), .. t4.AsReadOnlySpan(), .. t5.AsReadOnlySpan()]);
    public static NerveCacheKey Create<T1, T2, T3, T4, T5, T6>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5, in T6 t6)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged =>
            new([.. t1.AsReadOnlySpan(), .. t2.AsReadOnlySpan(), .. t3.AsReadOnlySpan(), .. t4.AsReadOnlySpan(), .. t5.AsReadOnlySpan(), .. t6.AsReadOnlySpan()]);
    public static NerveCacheKey Create<T1, T2, T3, T4, T5, T6, T7>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5, in T6 t6, in T7 t7)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged where T7 : unmanaged =>
            new([.. t1.AsReadOnlySpan(), .. t2.AsReadOnlySpan(), .. t3.AsReadOnlySpan(), .. t4.AsReadOnlySpan(), .. t5.AsReadOnlySpan(), .. t6.AsReadOnlySpan(), .. t7.AsReadOnlySpan()]);
}
