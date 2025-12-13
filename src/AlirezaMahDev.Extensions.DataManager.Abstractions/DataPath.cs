using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct DataPath(String64 Key,int Size, long Next, long Child, long Data,  long Index)
    : IDataDictionaryTree<DataPath, String64>, IDataValueDefault<DataPath>, IDataStorage<DataPath>, IDataIndex<DataPath>
{
    public static DataPath Default { get; } = new(default, -1, -1L, -1L, -1L,-1L);
}