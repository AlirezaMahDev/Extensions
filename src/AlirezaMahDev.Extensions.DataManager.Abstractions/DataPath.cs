using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct DataPath(String64 Key, int Size, DataOffset Next, DataOffset Child, DataOffset Data, DataOffset Index)
    : IDataDictionaryTree<DataPath, String64>, IDataValueDefault<DataPath>, IDataStorage<DataPath>, IDataIndex<DataPath>
{
    public String64 RefKey = Key;
    public int RefSize = Size;
    
    public DataOffset RefNext = Next;
    public DataOffset RefChild = Child;
    public DataOffset RefData = Data;
    public DataOffset RefIndex = Index;
    
    public static DataPath Default { get; } = new(default, -1, DataOffset.Null, DataOffset.Null, DataOffset.Null, DataOffset.Null);

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

    public DataOffset Next
    {
        readonly get => RefNext;
        set => RefNext = value;
    }

    public DataOffset Child
    {
        readonly get => RefChild;
        set => RefChild = value;
    }

    public DataOffset Data
    {
        readonly get => RefData;
        set => RefData = value;
    }

    public DataOffset Index
    {
        readonly get => RefIndex;
        set => RefIndex = value;
    }
    
    public int RefLock;
    public ref int Lock => ref Unsafe.AsRef(in this).RefLock;
}