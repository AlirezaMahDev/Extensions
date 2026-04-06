namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataAccess
{
    string Path { get; }

    IDataMap Map { get; }
    ref readonly DataLocation<DataPath> Root { get; }
    ref readonly DataLocation<DataTrash> Trash { get; }

    DataOffset AllocateOffset(int length);
    IDataAlive AllocationWithAlive(int length, out DataOffset offset);
    DataMapFilePartCacheAccess GetCache(in DataOffset offset, CancellationToken cancellationToken = default);

    IDataAlive GetCacheWithAlive(in DataOffset offset,
        out DataMapFilePartCacheAccess cache,
        CancellationToken cancellationToken = default);

    IDataAlive GetAlive(in DataOffset offset);

    void Flush();
}