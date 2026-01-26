using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public record struct DataTrash(DataOffset Child)
    : IDataCollection<DataTrash, DataTrashItem>, IDataValueDefault<DataTrash>
{
    public int Lock;
    public static DataTrash Default { get; } = new(DataOffset.Null);

    public ref int RefLock => ref Unsafe.AsRef(in this).Lock;
}