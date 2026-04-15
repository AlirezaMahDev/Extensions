using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.Logging;

namespace AlirezaMahDev.Extensions.DataManager;

internal sealed class DataMap : IDisposable, IDataMap
{
    private bool _disposed;

    internal readonly Memory<Lazy<DataMapFile>> Files;

    private long _allocSum;
    private readonly ReaderWriterLockSlim _flushLock = new();
    private readonly CancellationTokenSource _cleanTokenSource = new();
    private readonly Thread _thread;
    private readonly ILogger? _logger;

    public IDataStatus Status { get; }

    public bool CanAlloc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Volatile.Read(ref _allocSum) + DataDefaults.PartSize <= DataDefaults.AllocMax;
    }

    public bool MoreThanHighAlloc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Volatile.Read(ref _allocSum) > DataDefaults.AllocHigh;
    }

    public bool MoreThanNormalAlloc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Volatile.Read(ref _allocSum) > DataDefaults.AllocNormal;
    }

    public bool LessThanNormalAlloc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Volatile.Read(ref _allocSum) < DataDefaults.AllocNormal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMap(string path, ILogger? logger)
    {
        _logger = logger;
        Status = new DataStatus(this);

        var directoryPath = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        Files =
            new([
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
            ]);

        _thread = new Thread(CleanWorker);
        _thread.Start(this);
    }

    private static void CleanWorker(object? parameter)
    {
        var dataMap = (DataMap?)parameter ?? throw new ArgumentNullException(nameof(parameter));
        SpinWait spinWait = default;
        while (!dataMap._cleanTokenSource.IsCancellationRequested)
        {
            spinWait.SpinOnce();

            if (!dataMap.MoreThanNormalAlloc)
            {
                continue;
            }

            dataMap._flushLock.EnterReadLock();
            try
            {
                if (!dataMap.MoreThanNormalAlloc)
                {
                    continue;
                }

                for (var fileId = 0; fileId < DataDefaults.FileCount; fileId++)
                {
                    var file = dataMap.Files.Span[fileId];
                    if (!file.IsValueCreated)
                    {
                        continue;
                    }

                    if (file.Value.Clean(() => dataMap.MoreThanHighAlloc) && dataMap.LessThanNormalAlloc)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                if (dataMap._logger is not null)
                {
                    dataMap._logger.LogError(e, "CleanWorker failed");
                }
                else
                {
                    Console.Error.WriteLine(e);
                }
            }
            finally
            {
                dataMap._flushLock.ExitReadLock();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Alloc()
    {
        SpinWait spinWait = default;
        while (!_disposed)
        {
            var allocSum = Volatile.Read(ref _allocSum);
            var newAllocSum = allocSum + DataDefaults.PartSize;
            if (newAllocSum <= DataDefaults.AllocMax &&
                Interlocked.CompareExchange(ref _allocSum, newAllocSum, allocSum) == allocSum)
            {
                return;
            }

            spinWait.SpinOnce();
        }

        throw new ObjectDisposedException(nameof(DataMap));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Free()
    {
        Interlocked.Add(ref _allocSum, -DataDefaults.PartSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMapFile File(in DataOffset offset)
    {
        _flushLock.EnterReadLock();
        try
        {
            return Files.Span[offset.FileId].Value;
        }
        finally
        {
            _flushLock.ExitReadLock();
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Flush()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _flushLock.EnterWriteLock();
        try
        {
            for (var fileId = 0; fileId < DataDefaults.FileCount; fileId++)
            {
                var file = Files.Span[fileId];
                if (!file.IsValueCreated)
                {
                    continue;
                }

                file.Value.Flush();
            }
        }
        finally
        {
            _flushLock.ExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _disposed = true;
        _cleanTokenSource.Cancel();
        _cleanTokenSource.Dispose();
        _thread.Join();
        _flushLock.Dispose();

        for (int i = 0; i < Files.Length; i++)
        {
            var file = Files.Span[i];
            if (!file.IsValueCreated)
            {
                continue;
            }

            file.Value.Dispose();
        }
    }
}