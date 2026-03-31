namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct DataPath(
    String64 key,
    int size,
    DataOffset next,
    DataOffset child,
    DataOffset data,
    DataOffset index)
    : IDataDictionaryTree<DataPath, String64>, IDataValueDefault<DataPath>, IDataStorage<DataPath>
{
    private String64 _key = key;
    private DataOffset _next = next;
    private DataOffset _child = child;
    private DataOffset _data = data;
    private DataOffset _index = index;
    private int _size = size;
    private DataLock _lock;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool Equals(scoped ref readonly DataPath other)
    {
        return _key == other._key;
    }

    private static readonly DataPath DefaultField =
        new(default, -1, DataOffset.Null, DataOffset.Null, DataOffset.Null, DataOffset.Null);

    public static ref readonly DataPath Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref DefaultField;
    }

    public readonly ref String64 Key
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._key;
    }

    public readonly ref int Size => ref Unsafe.AsRef(in this)._size;

    public readonly ref DataOffset Next
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._next;
    }

    public readonly ref DataOffset Child
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._child;
    }

    public readonly ref DataOffset Data
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._data;
    }

    public readonly ref DataOffset Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._index;
    }

    public readonly ref DataLock Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._lock;
    }
}