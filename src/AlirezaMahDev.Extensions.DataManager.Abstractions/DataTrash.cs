namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct DataTrash(in DataOffset child)
    : IDataCollection<DataTrash, DataTrashItem>, IDataValueDefault<DataTrash>
{
    private DataOffset _child = child;
    private int _lock;
    public static readonly DataTrash DefaultField = new(DataOffset.Null);

    public static ref readonly DataTrash Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref DefaultField;
    }

    public ref int Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this)._lock;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(in DataTrash other)
    {
        return _child == other.Child;
    }

    public ref DataOffset Child
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._child;
    }
}