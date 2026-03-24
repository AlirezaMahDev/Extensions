namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct DataPath(
    in String64 key,
    in int size,
    in DataOffset next,
    in DataOffset child,
    in DataOffset data,
    in DataOffset index)
    : IDataDictionaryTree<DataPath, String64>, IDataValueDefault<DataPath>, IDataStorage<DataPath>
{
    private String64 _key = key;
    private DataOffset _next = next;
    private DataOffset _child = child;
    private DataOffset _data = data;
    private DataOffset _index = index;
    private int _size = size;
    private int _lock;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(in DataPath other)
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

    public ref String64 Key
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._key;
    }

    public ref int Size
    {
        get => ref Unsafe.AsRef(in this)._size;
    }

    public ref DataOffset Next
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._next;
    }

    public ref DataOffset Child
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._child;
    }

    public ref DataOffset Data
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._data;
    }

    public ref DataOffset Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._index;
    }

    public ref int Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._lock;
    }
}