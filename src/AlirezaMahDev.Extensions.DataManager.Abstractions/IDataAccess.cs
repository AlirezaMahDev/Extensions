namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataAccess
{
    string Path { get; }

    DataLocation<DataPath> Root { get; }
    DataWrap<DataPath> RootWrap { get; }

    DataLocation<DataTrash> GetTrash();
    ValueTask<DataLocation<DataTrash>> GetTrashAsync(CancellationToken cancellationToken = default);

    AllocateMemory<byte> AllocateMemory(int length);
    Memory<byte> ReadMemory(long offset, int length);
    void Flush();

    DataLockOffsetDisposable Lock(long offset);
    Task<DataLockOffsetDisposable> LockAsync(long offset, CancellationToken cancellationToken = default);
}
