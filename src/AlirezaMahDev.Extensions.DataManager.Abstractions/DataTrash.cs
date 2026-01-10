using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct DataTrash(DataOffset Child)
    : IDataCollection<DataTrash, DataTrashItem>, IDataValueDefault<DataTrash>
{
    public int RefLock;
    public static DataTrash Default { get; } = new(DataOffset.Null);

    public ref int Lock => ref Unsafe.AsRef(in this).RefLock;
}