using System.Runtime.Intrinsics;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct DataTrashItem(DataOffset offset, DataOffset next)
    : IDataCollectionItem<DataTrashItem>, IDataValueDefault<DataTrashItem>
{
    private DataOffset _offset = offset;
    private DataOffset _next = next;
    private DataLock _lock;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool Equals(scoped ref readonly DataTrashItem other)
    {
        return Vector256.LoadUnsafe(ref Unsafe.As<DataTrashItem, byte>(ref Unsafe.AsRef(in this))) ==
               Vector256.LoadUnsafe(ref Unsafe.As<DataTrashItem, byte>(ref Unsafe.AsRef(in other)));
    }

    private static readonly DataTrashItem DefaultField = new(DataOffset.Null, DataOffset.Null);

    public static ref readonly DataTrashItem Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref DefaultField;
    }


    public readonly ref DataLock Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._lock;
    }

    public readonly ref DataOffset Offset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._offset;
    }

    public readonly ref DataOffset Next
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._next;
    }
}