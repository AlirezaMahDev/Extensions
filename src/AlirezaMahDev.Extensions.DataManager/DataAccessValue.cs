using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

[StructLayout(LayoutKind.Sequential)]
internal struct DataAccessValue : IDataValue<DataAccessValue>
{
    public long LastOffset;
    private DataLock _lock;

    public readonly ref DataLock Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._lock;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool Equals(scoped ref readonly DataAccessValue other)
    {
        return LastOffset == other.LastOffset;
    }
}