using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public record struct DataTrashItem(DataOffset Offset, DataOffset Next)
    : IDataCollectionItem<DataTrashItem>, IDataValueDefault<DataTrashItem>
{
    public static DataTrashItem Default { get; } = new(DataOffset.Null, DataOffset.Null);

    public int Lock;
    public ref int RefLock => ref Unsafe.AsRef(in this).Lock;
}