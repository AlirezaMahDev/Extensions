using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

class DataAccess : IDisposable, IDataAccess
{
    public string Path { get; }

    private bool _disposedValue;
    private readonly DataMap _map;
    private readonly DataLock _lock = new();
    private readonly DataLocation<DataAccessValue> _value;

    public ref long LastOffset => ref _value.RefValue.LastOffset;

    public DataAccess(string path)
    {
        Path = path;

        _map = new(System.IO.Path.Combine(Path, DataDefaults.FileFormat));
        _value = this.Read<DataAccessValue>(0);

        if (LastOffset == 0)
        {
            LastOffset = _value.Length;
            this.Create<DataPath>();
        }

        Flush();
    }

    private long AllocateOffset(int length)
    {
        if (length > DataDefaults.PartSize)
            throw new Exception($"length > {DataDefaults.PartSize}");

        long offset;
        long lastOffset;
        do
        {
            offset = LastOffset;
            lastOffset = LastOffset;

            if (DataHelper.PartIndex(offset) != DataHelper.PartIndex(offset + length - 1))
                offset += DataDefaults.PartSize - (offset % DataDefaults.PartSize);
            if (DataHelper.FileId(offset) != DataHelper.FileId(offset + length - 1))
                offset += DataDefaults.FileSize - (offset % DataDefaults.FileSize);

        } while (Interlocked.CompareExchange(ref LastOffset, offset + length, lastOffset) != lastOffset);

        return offset;
    }

    private DataPart AccessPart(long offset) =>
        _map.File(offset).Part(offset);

    public DataLocation<DataPath> GetRoot() =>
        this.Read<DataPath>(_value.Length);

    public async ValueTask<DataLocation<DataPath>> GetRootAsync(CancellationToken cancellationToken = default) =>
        await this.ReadAsync<DataPath>(_value.Length, cancellationToken);


    public DataLocation<DataTrash> GetTrash()
    {
        return GetRoot()
            .Wrap(x => x.TreeDictionary())
            .GetOrAdd(".trash")
            .Wrap(x => x.Storage())
            .GetOrCreateData<DataPath, DataTrash>();
    }

    public async ValueTask<DataLocation<DataTrash>> GetTrashAsync(CancellationToken cancellationToken = default)
    {
        var root = await GetRootAsync(cancellationToken);
        var trashPath = await root.Wrap(x => x.TreeDictionary())
            .GetOrAddAsync(".trash", cancellationToken);
        return await trashPath
            .Wrap(x => x.Storage())
            .GetOrCreateDataAsync<DataPath, DataTrash>(cancellationToken);
    }

    public AllocateMemory<byte> AllocateMemory(int length)
    {
        var offset = AllocateOffset(length);

        var part = AccessPart(offset);
        var partOffset = DataHelper.PartOffset(offset);
        var memory = part.Memory[partOffset..(partOffset + length)];

        return new(offset, memory);
    }

    public ValueTask<AllocateMemory<byte>> AllocateMemoryAsync(int length, CancellationToken cancellationToken = default)
    {
        return cancellationToken.IsCancellationRequested
            ? ValueTask.FromCanceled<AllocateMemory<byte>>(cancellationToken)
            : ValueTask.FromResult(AllocateMemory(length));
    }

    public Memory<byte> ReadMemory(long offset, int length)
    {
        var part = AccessPart(offset);
        var partOffset = DataHelper.PartOffset(offset);
        var memory = part.Memory[partOffset..(partOffset + length)];

        return memory;
    }

    public ValueTask<Memory<byte>> ReadMemoryAsync(long offset,
        int length,
        CancellationToken cancellationToken = default)
    {
        return cancellationToken.IsCancellationRequested
            ? ValueTask.FromCanceled<Memory<byte>>(cancellationToken)
            : ValueTask.FromResult(ReadMemory(offset, length));
    }

    public void Flush()
    {
        _map.Flush();
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        await _map.FlushAsync(cancellationToken);
    }

    public DataLockOffsetDisposable Lock(long offset)
    {
        return _lock.GetOrAdd(offset).Lock();
    }

    public async Task<DataLockOffsetDisposable> LockAsync(long offset, CancellationToken cancellationToken = default)
    {
        return await _lock.GetOrAdd(offset).LockAsync(cancellationToken);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _map.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}