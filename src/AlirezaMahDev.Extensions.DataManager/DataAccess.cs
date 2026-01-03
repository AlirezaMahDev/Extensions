using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

class DataAccess : IDisposable, IDataAccess
{
    public string Path { get; }

    private bool _disposedValue;
    private readonly DataMap _map;
    private readonly DataLock _lock = new();
    private readonly DataLocation<DataAccessValue> _value;

    public ref long LastOffset => ref _value.GetRefValue(this).LastOffset;

    public DataAccess(string path)
    {
        Path = path;

        _map = new(System.IO.Path.Combine(Path, DataDefaults.FileFormat));
        _value = new(0);

        if (LastOffset == 0)
        {
            LastOffset = _value.Length;
            this.Create<DataPath>();
        }

        Flush();

        Root = new(_value.Length);
    }

    private long AllocateOffset(int length)
    {
        if (length > DataDefaults.PartSize)
            throw new($"length > {DataDefaults.PartSize}");

        long offset;
        long lastOffset;
        do
        {
            lastOffset = LastOffset;
            offset = lastOffset;

            if (DataHelper.PartIndex(offset) != DataHelper.PartIndex(offset + length - 1))
                offset += DataDefaults.PartSize - (offset % DataDefaults.PartSize);
            if (DataHelper.FileId(offset) != DataHelper.FileId(offset + length - 1))
                offset += DataDefaults.FileSize - (offset % DataDefaults.FileSize);
        } while (Interlocked.CompareExchange(ref LastOffset, offset + length, lastOffset) != lastOffset);

        return offset;
    }

    private DataMapFilePart AccessPart(long offset) =>
        _map.File(offset).Part(offset);

    public DataLocation<DataPath> Root { get; }
    public DataWrap<DataPath> RootWrap => Root.Wrap(this);

    public DataLocation<DataTrash> GetTrash()
    {
        return Root
            .Wrap(this, x => x.TreeDictionary())
            .GetOrAdd(".trash")
            .Wrap(this, x => x.Storage())
            .GetOrCreateData<DataPath, DataTrash>();
    }

    public async ValueTask<DataLocation<DataTrash>> GetTrashAsync(CancellationToken cancellationToken = default)
    {
        var trashPath = await Root
            .Wrap(this, x => x.TreeDictionary())
            .GetOrAddAsync(".trash", cancellationToken);
        return trashPath
            .Wrap(this, x => x.Storage())
            .GetOrCreateData<DataPath, DataTrash>();
    }

    public AllocateMemory<byte> AllocateMemory(int length)
    {
        var offset = AllocateOffset(length);

        var part = AccessPart(offset);
        var partOffset = DataHelper.PartOffset(offset);
        var memory = part.Memory.Slice(partOffset, length);

        return new(offset, memory);
    }

    public Memory<byte> ReadMemory(long offset, int length)
    {
        var part = AccessPart(offset);
        var partOffset = DataHelper.PartOffset(offset);
        var memory = part.Memory.Slice(partOffset, length);

        return memory;
    }

    public void Flush()
    {
        _map.Flush();
    }

    public DataLockOffsetDisposable Lock(long offset)
    {
        return _lock.GetOrAdd(offset).Lock();
    }

    public async Task<DataLockOffsetDisposable> LockAsync(long offset,
        CancellationToken cancellationToken = default)
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