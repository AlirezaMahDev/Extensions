using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct NerveCacheIdKey(UInt128 id, UInt128 key)
{
    public readonly UInt128 Id = id;
    public readonly UInt128 Key = key;
}