using System.Diagnostics;
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

    public ref long LastOffset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _value.GetRefValue(this).LastOffset;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataAccess(string path)
    {
        Path = path;

        _map = new(System.IO.Path.Combine(Path, DataDefaults.FileFormat));
        _value = new(DataOffset.Create(0, sizeof(long)));

        if (LastOffset == 0)
        {
            LastOffset = _value.Offset.Length;
            this.Create<DataPath>(out _);
        }

        Flush();

        Root = new(DataOffset.Create(_value.Offset.Length, Unsafe.SizeOf<DataPath>()));
        RootWrap = Root.Wrap(this);
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
            lastOffset = LastOffset;
            offset = lastOffset;

            if (DataHelper.PartIndex(offset) != DataHelper.PartIndex(offset + length - 1))
            {
                offset += DataDefaults.PartSize - (offset % DataDefaults.PartSize);
            }

            if (DataHelper.FileId(offset) != DataHelper.FileId(offset + length - 1))
            {
                offset += DataDefaults.FileSize - (offset % DataDefaults.FileSize);
            }
        } while (Interlocked.CompareExchange(ref LastOffset, offset + length, lastOffset) != lastOffset);

        return DataOffset.Create(offset, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataMapFilePart AccessPart(in DataOffset offset)
    {
        return _map.File(in offset).Part(in offset);
    }

    public DataLocation<DataPath> Root
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    public DataWrap<DataPath> RootWrap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataLocation<DataTrash> GetTrash()
    {
        return Root
            .Wrap(this, x => x.TreeDictionary())
            .GetOrAdd(".trash")
            .Wrap(this, x => x.Storage())
            .GetOrCreateData<DataPath, DataTrash>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Memory<byte> ReadMemory(in DataOffset offset)
    {
        return AccessPart(in offset).Memory.Slice(offset.Offset, offset.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ref byte ReadRef(in DataOffset offset)
    {
        if ((uint)offset.FileId >= DataDefaults.FileCount ||
            (uint)offset.PartIndex >= DataDefaults.PartCount)
        {
            Debug.WriteLine($"CORRUPT OFFSET: FileId={offset.FileId}, " +
                            $"PartIndex={offset.PartIndex}, " +
                            $"Offset={offset.Offset}, " +
                            $"Length={offset.Length}");
            Debugger.Break();
        }

        return ref AccessPart(in offset).GetRef(offset.Offset);
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