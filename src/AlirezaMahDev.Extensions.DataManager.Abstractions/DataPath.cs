using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct DataPath(String64 Key, int Size, long Next, long Child, long Data, long Index)
    : IDataDictionaryTree<DataPath, String64>, IDataValueDefault<DataPath>, IDataStorage<DataPath>, IDataIndex<DataPath>
{
    public String64 RefKey = Key;
    public int RefSize = Size;
    public long RefNext = Next;
    public long RefChild = Child;
    public long RefData = Data;
    public long RefIndex = Index;

    public static DataPath Default { get; } = new(default, -1, -1L, -1L, -1L, -1L);

    public String64 Key
    {
        readonly get => RefKey;
        set => RefKey = value;
    }

    public int Size
    {
        readonly get => RefSize;
        set => RefSize = value;
    }

    public long Next
    {
        readonly get => RefNext;
        set => RefNext = value;
    }

    public long Child
    {
        readonly get => RefChild;
        set => RefChild = value;
    }

    public long Data
    {
        readonly get => RefData;
        set => RefData = value;
    }

    public long Index
    {
        readonly get => RefIndex;
        set => RefIndex = value;
    }
}