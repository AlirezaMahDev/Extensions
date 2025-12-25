using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Win32.SafeHandles;

namespace AlirezaMahDev.Extensions.DataManager;

class DataAccess : IDisposable, IDataAccess
{
    public string Path { get; }

    private bool _disposedValue;

    private readonly ConcurrentDictionary<long, DataMemory> _cache = [];
    private long _length;

    private readonly SafeFileHandle _safeFileHandle;

    public DataAccess(string path)
    {
        Path = path;
        _safeFileHandle = File.OpenHandle(Path,
            FileMode.OpenOrCreate,
            FileAccess.ReadWrite,
            FileShare.None);
        _length = RandomAccess.GetLength(_safeFileHandle);
        if (_length == 0)
        {
            this.Create<DataPath>();
        }

        Save();
    }

    private long AllocateOffset(int length)
    {
        return Interlocked.Add(ref _length, length) - length;
    }

    public DataLocation<DataPath> GetRoot() =>
        this.Read<DataPath>(0);

    public async ValueTask<DataLocation<DataPath>> GetRootAsync(CancellationToken cancellationToken = default) =>
        await this.ReadAsync<DataPath>(0, cancellationToken);


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

    public AllocateMemory AllocateMemory(int length)
    {
        var offset = AllocateOffset(length);
        var dataMemory = new DataMemory(length);
        _cache.TryAdd(offset, dataMemory);
        dataMemory.CreateChangePoint();
        return new(offset, dataMemory.Memory);
    }

    public ValueTask<AllocateMemory> AllocateMemoryAsync(int length, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<AllocateMemory>(cancellationToken);
        }

        var offset = AllocateOffset(length);
        var dataMemory = new DataMemory(length);
        _cache.TryAdd(offset, dataMemory);
        dataMemory.CreateChangePoint();
        return ValueTask.FromResult(new AllocateMemory(offset, dataMemory.Memory));
    }

    public Memory<byte> ReadMemory(long offset, int length)
    {
        if (_cache.TryGetValue(offset, out var dataMemory))
        {
            return dataMemory.Memory;
        }

        if (_cache.TryAdd(offset, dataMemory = new(length)))
        {
            RandomAccess.Read(_safeFileHandle, dataMemory.Memory.Span, offset);
            dataMemory.CreateChangePoint();
            return dataMemory.Memory;
        }

        return _cache[offset].Memory;
    }

    public async ValueTask<Memory<byte>> ReadMemoryAsync(long offset,
        int length,
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(offset, out var dataMemory))
        {
            return dataMemory.Memory;
        }

        if (_cache.TryAdd(offset, dataMemory = new(length)))
        {
            await RandomAccess.ReadAsync(_safeFileHandle, dataMemory.Memory, offset, cancellationToken);
            dataMemory.CreateChangePoint();
            return dataMemory.Memory;
        }

        return _cache[offset].Memory;
    }

    public void WriteMemory(long offset, Memory<byte> memory)
    {
        RandomAccess.Write(_safeFileHandle, memory.Span, offset);
    }

    public async ValueTask WriteMemoryAsync(long offset,
        Memory<byte> memory,
        CancellationToken cancellationToken = default)
    {
        await RandomAccess.WriteAsync(_safeFileHandle, memory, offset, cancellationToken);
    }

    public void Save()
    {
        Parallel.ForEach(_cache,
            (pair, _) =>
            {
                if (pair.Value.HasChanged)
                {
                    WriteMemory(pair.Key, pair.Value.Memory);
                    pair.Value.CreateChangePoint();
                }
            });
        RandomAccess.FlushToDisk(_safeFileHandle);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await Parallel.ForEachAsync(_cache,
            cancellationToken,
            async ValueTask (pair, token) =>
            {
                if (pair.Value.HasChanged)
                {
                    await WriteMemoryAsync(pair.Key, pair.Value.Memory, token);
                    pair.Value.CreateChangePoint();
                }
            });
        RandomAccess.FlushToDisk(_safeFileHandle);
    }

    private SemaphoreSlim GetSemaphoreSlim(long offset) =>
        _cache[offset].SemaphoreSlim;

    public LockScope LockScope(long offset)
    {
        var semaphoreSlim = GetSemaphoreSlim(offset);
        semaphoreSlim.Wait();
        return new(semaphoreSlim);
    }

    public async Task<LockScope> LockScopeAsync(long offset, CancellationToken cancellationToken = default)
    {
        var semaphoreSlim = GetSemaphoreSlim(offset);
        await semaphoreSlim.WaitAsync(cancellationToken);
        return new(semaphoreSlim);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _safeFileHandle.Dispose();
                foreach (var keyValuePair in _cache)
                {
                    keyValuePair.Value.Dispose();
                }
            }

            _cache.Clear();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}