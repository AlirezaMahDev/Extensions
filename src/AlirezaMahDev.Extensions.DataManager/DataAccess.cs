using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.Logging;

namespace AlirezaMahDev.Extensions.DataManager;

internal class DataAccess : IDisposable, IDataAccess
{
    public string Path
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    private bool _disposedValue;
    private readonly DataMap _map;
    private readonly DataLocation<DataAccessValue> _value;
    private readonly DataLocation<DataPath> _root;
    private readonly DataLocation<DataTrash> _trash;

    public IDataMap Map
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => _map;
    }

    public ref readonly DataLocation<DataPath> Root
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _root;
    }

    public ref readonly DataLocation<DataTrash> Trash
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _trash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataAccess(string path, ILogger<DataAccess>? logger)
    {
        Path = path;

        _map = new(System.IO.Path.Combine(Path, DataDefaults.FileFormat), logger);
        DataLocation<DataAccessValue>.Read(this, DataOffset.Create(0, Unsafe.SizeOf<DataAccessValue>()), out _value);

        if (_value.ReadLock((scoped ref readonly x) => x.LastOffset == 0))
        {
            _value.WriteLock((scoped ref x) => x.LastOffset += _value.Offset.Length);
            this.Create<DataPath>(out _);
        }

        Flush();

        DataLocation<DataPath>.Read(this,
            DataOffset.Create(_value.Offset.Length, Unsafe.SizeOf<DataPath>()),
            out _root);

        _trash = Root.Wrap(x => x.Dictionary())
            .GetOrAdd(".trash")
            .Wrap(x => x.Storage())
            .GetOrCreateData<DataPath, DataTrash>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset AllocateOffset(int length)
    {
        if (length > DataDefaults.PartSize)
        {
            throw new($"length > {DataDefaults.PartSize}");
        }

        long offset = _value.WriteLock((scoped ref value) =>
        {
            long offset;
            long lastOffset;
            do
            {
                lastOffset = Interlocked.Read(ref value.LastOffset);
                offset = lastOffset;

                if (DataHelper.PartIndex(offset) != DataHelper.PartIndex(offset + length - 1))
                {
                    offset += DataDefaults.PartSize - (offset % DataDefaults.PartSize);
                }

                if (DataHelper.FileId(offset) != DataHelper.FileId(offset + length - 1))
                {
                    offset += DataDefaults.FileSize - (offset % DataDefaults.FileSize);
                }
            } while (Interlocked.CompareExchange(
                         ref value.LastOffset,
                         offset + length,
                         lastOffset) !=
                     lastOffset);

            return offset;
        });

        return DataOffset.Create(offset, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public IDataAlive AllocationWithAlive(int length, out DataOffset offset)
    {
        offset = AllocateOffset(length);
        return GetAlive(offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMapFilePartCacheAccess GetCache(in DataOffset offset, CancellationToken cancellationToken = default)
    {
        return _map.File(in offset).Part(in offset).GetCache(cancellationToken);
    }

    public IDataAlive GetCacheWithAlive(in DataOffset offset,
        out DataMapFilePartCacheAccess cache,
        CancellationToken cancellationToken = default)
    {
        var part = _map.File(in offset).Part(in offset);
        var dataAlive = part.GetAlive();
        cache = part.GetCache(cancellationToken);
        return dataAlive;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public IDataAlive GetAlive(in DataOffset offset)
    {
        return _map.File(in offset).Part(in offset).GetAlive();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Flush()
    {
        _map.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}