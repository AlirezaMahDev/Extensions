namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct CacheValue : IDataValueDefault<CacheValue>, IDataValue<CacheValue>, IDataStorage<CacheValue>
{
    public static readonly CacheValue DefaultField = new()
    {
        _data = default
    };

    public static ref readonly CacheValue Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref DefaultField;
    }

    private DataOffset _data;

    private DataLock _lock;

    public readonly ref DataLock Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._lock;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool Equals(scoped ref readonly CacheValue other)
    {
        return _data.Equals(in other.Data);
    }

    public readonly ref DataOffset Data
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._data;
    }

    public override readonly string ToString()
    {
        return $"{Data}";
    }
}