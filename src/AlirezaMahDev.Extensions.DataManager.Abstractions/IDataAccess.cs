namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataAccess
{
    string Path { get; }

    DataLocation<DataPath> Root { get; }
    DataWrap<DataPath> RootWrap { get; }

    DataLocation<DataTrash> GetTrash();
    ValueTask<DataLocation<DataTrash>> GetTrashAsync(CancellationToken cancellationToken = default);

    AllocateMemory<byte> AllocateMemory(int length);
    Memory<byte> ReadMemory(DataOffset offset);
    void Flush();
}