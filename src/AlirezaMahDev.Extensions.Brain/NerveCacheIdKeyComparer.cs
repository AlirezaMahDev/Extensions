using System.IO.Hashing;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

using FASTER.core;

namespace AlirezaMahDev.Extensions.Brain;

public struct NerveCacheIdKeyComparer : IFasterEqualityComparer<NerveCacheIdKey>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ref NerveCacheIdKey k1, ref NerveCacheIdKey k2)
        => Vector256.LoadUnsafe(ref Unsafe.As<NerveCacheIdKey, byte>(ref k1)) ==
           Vector256.LoadUnsafe(ref Unsafe.As<NerveCacheIdKey, byte>(ref k2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetHashCode64(ref NerveCacheIdKey key)
    {
        return (long)XxHash3.HashToUInt64(MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref key, 1)));
    }
}