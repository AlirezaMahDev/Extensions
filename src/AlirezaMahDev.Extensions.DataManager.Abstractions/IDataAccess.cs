namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataAccess
{
    string Path { get; }

    DataLocation<DataPath> GetRoot();
    ValueTask<DataLocation<DataPath>> GetRootAsync(CancellationToken cancellationToken = default);

    DataLocation<DataTrash> GetTrash();
    ValueTask<DataLocation<DataTrash>> GetTrashAsync(CancellationToken cancellationToken = default);

    AllocateMemory<byte> AllocateMemory(int length);
    ValueTask<AllocateMemory<byte>> AllocateMemoryAsync(int length, CancellationToken cancellationToken = default);

    Memory<byte> ReadMemory(long offset, int length);
    ValueTask<Memory<byte>> ReadMemoryAsync(long offset, int length, CancellationToken cancellationToken = default);

    void Flush();
    ValueTask FlushAsync(CancellationToken cancellationToken = default);

    DataLockOffsetDisposable Lock(long offset);
    Task<DataLockOffsetDisposable> LockAsync(long offset, CancellationToken cancellationToken = default);
}
