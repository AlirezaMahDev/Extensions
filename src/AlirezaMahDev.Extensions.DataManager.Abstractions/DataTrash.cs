namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct DataTrash(DataOffset child)
    : IDataCollection<DataTrash, DataTrashItem>, IDataValueDefault<DataTrash>
{
    private DataOffset _child = child;
    private DataLock _lock;
    public static readonly DataTrash DefaultField = new(DataOffset.Null);

    public static ref readonly DataTrash Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref DefaultField;
    }

    public readonly ref DataLock Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._lock;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool Equals(scoped ref readonly DataTrash other)
    {
        return _child == other.Child;
    }

    public readonly ref DataOffset Child
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._child;
    }
}