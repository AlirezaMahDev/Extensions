using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct DataTrashItem(DataOffset Offset, DataOffset Next)
    : IDataCollectionItem<DataTrashItem>, IDataValueDefault<DataTrashItem>
{
    public static DataTrashItem Default { get; } = new(DataOffset.Null, DataOffset.Null);

    public int RefLock;
    public ref int Lock => ref Unsafe.AsRef(in this).RefLock;
}