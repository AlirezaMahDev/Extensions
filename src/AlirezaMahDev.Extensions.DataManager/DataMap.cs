using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

internal sealed class DataMap : IDisposable, IDataMap
{
    private bool _disposed;

    internal readonly Lazy<DataMapFile>[] Files;

    private long _allocSum;
    private readonly ReaderWriterLockSlim _cleanLock = new();

    public IDataStatus Status { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMap(string path)
    {
        Status = new DataStatus(this);

        var directoryPath = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        Files =
        [
            .. Enumerable.Range(0, DataDefaults.FileCount)
                .Select(id =>
                    new Lazy<DataMapFile>(() =>
                            new(this,
                                System.IO.File.OpenHandle(string.Format(path, id),
                                    FileMode.OpenOrCreate,
                                    FileAccess.ReadWrite,
                                    FileShare.None,
                                    FileOptions.RandomAccess)),
                        LazyThreadSafetyMode.ExecutionAndPublication)
                )
        ];
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Alloc()
    {
        while (!_disposed)
        {
            var allocSum = Volatile.Read(ref _allocSum);
            var newAllocSum = allocSum + DataDefaults.PartSize;
            if (newAllocSum <= DataDefaults.AllocMax &&
                Interlocked.CompareExchange(ref _allocSum, newAllocSum, allocSum) == allocSum)
            {
                return;
            }
            else
            {
                Clean();
            }
        }

        throw new ObjectDisposedException(nameof(DataMap));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _cleanLock.EnterReadLock();
        try
        {
            if (Volatile.Read(ref _allocSum) + DataDefaults.PartSize <= DataDefaults.AllocMax)
            {
                return;
            }
        }
        finally
        {
            _cleanLock.ExitReadLock();
        }

        _cleanLock.EnterUpgradeableReadLock();
        try
        {
            if (Volatile.Read(ref _allocSum) + DataDefaults.PartSize <= DataDefaults.AllocMax)
            {
                return;
            }

            Flush();
        }
        finally
        {
            _cleanLock.ExitUpgradeableReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Free()
    {
        Interlocked.Add(ref _allocSum, -DataDefaults.PartSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMapFile File(in DataOffset offset)
    {
        _cleanLock.EnterReadLock();
        try
        {
            return Files[offset.FileId].Value;
        }
        finally
        {
            _cleanLock.ExitReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Flush()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _cleanLock.EnterWriteLock();
        try
        {
            for (var fileId = 0; fileId < DataDefaults.FileCount; fileId++)
            {
                var file = Files[fileId];
                if (!file.IsValueCreated)
                {
                    continue;
                }

                file.Value.Flush();
            }
        }
        finally
        {
            _cleanLock.ExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (var dataFile in Files.Where(x => x.IsValueCreated))
        {
            dataFile.Value.Dispose();
        }

        _disposed = true;
    }
}