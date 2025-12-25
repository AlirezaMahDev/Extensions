namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataAccess
{
    string Path { get; }

    DataLocation<DataPath> GetRoot();
    ValueTask<DataLocation<DataPath>> GetRootAsync(CancellationToken cancellationToken = default);

    DataLocation<DataTrash> GetTrash();
    ValueTask<DataLocation<DataTrash>> GetTrashAsync(CancellationToken cancellationToken = default);

    AllocateMemory AllocateMemory(int length);

    ValueTask<AllocateMemory> AllocateMemoryAsync(int length,
        CancellationToken cancellationToken = default);

    Memory<byte> ReadMemory(long offset, int length);

    ValueTask<Memory<byte>> ReadMemoryAsync(long offset,
        int length,
        CancellationToken cancellationToken = default);

    void WriteMemory(long offset, Memory<byte> memory);
    ValueTask WriteMemoryAsync(long offset, Memory<byte> memory, CancellationToken cancellationToken = default);

    void Save();
    Task SaveAsync(CancellationToken cancellationToken = default);

    // public void Flush();
    // public ValueTask FlushAsync(CancellationToken cancellationToken = default);

    LockScope LockScope(long offset);
    Task<LockScope> LockScopeAsync(long offset, CancellationToken cancellationToken = default);
}