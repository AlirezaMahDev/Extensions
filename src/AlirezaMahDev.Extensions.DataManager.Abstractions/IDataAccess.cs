namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataAccess
{
    string Path { get; }

    DataLocation<DataPath> Root { get; }
    DataWrap<DataPath> RootWrap { get; }

    DataLocation<DataTrash> GetTrash();
    
    DataOffset AllocateOffset(int length);
    Memory<byte> ReadMemory(in DataOffset offset);
    ref byte ReadRef(in DataOffset offset);
    void Flush();
}