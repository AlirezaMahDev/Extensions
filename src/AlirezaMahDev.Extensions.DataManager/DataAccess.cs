using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

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
    public DataAccess(string path)
    {
        Path = path;

        _map = new(System.IO.Path.Combine(Path, DataDefaults.FileFormat));
        DataLocation<DataAccessValue>.Read(this, DataOffset.Create(0, sizeof(long)), out _value);

        if (_value.ReadLock((scoped ref readonly x) => x.LastOffset == 0))
        {
            _value.WriteLock((scoped ref x) => x.LastOffset += _value.Offset.Length);
            this.Create<DataPath>(out _);
        }

        Flush();

        DataLocation<DataPath>.Read(this,
            DataOffset.Create(_value.Offset.Length, Unsafe.SizeOf<DataPath>()),
            out _root);

        _trash = Root.Wrap(this, x => x.TreeDictionary())
            .GetOrAdd(".trash")
            .Wrap(this, x => x.Storage())
            .GetOrCreateData<DataPath, DataTrash>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset AllocateOffset(int length)
    {
        if (length > DataDefaults.PartSize)
        {
            throw new($"length > {DataDefaults.PartSize}");
        }

        long offset;
        long lastOffset;
        do
        {
            lastOffset = Volatile.Read(ref _value.UnsafeRefValue.LastOffset);
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
            ref _value.UnsafeRefValue.LastOffset,
            offset + length,
            lastOffset) != lastOffset);

        return DataOffset.Create(offset, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataMapFilePart AccessPart(ref DataOffset offset)
    {
        return _map.File(ref offset).Part(ref offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public IDataMapFilePartOwner GetOwner(ref DataOffset offset)
    {
        return AccessPart(ref offset).GetOwner();
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