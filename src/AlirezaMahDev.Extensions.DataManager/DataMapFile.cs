using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Win32.SafeHandles;

namespace AlirezaMahDev.Extensions.DataManager;

internal sealed class DataMapFile : IDisposable
{
    private bool _disposed;

    internal readonly Lazy<DataMapFilePart>[] Parts;
    private readonly SafeFileHandle _safeFileHandle;
    private readonly ReaderWriterLockSlim _cleanLock = new();


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMapFile(DataMap map, SafeFileHandle safeFileHandle)
    {
        Map = map;
        _safeFileHandle = safeFileHandle;
        Parts =
        [
            .. Enumerable.Range(0, DataDefaults.PartCount)
                .Select(index => new Lazy<DataMapFilePart>(() =>
                        new(this,safeFileHandle, index * DataDefaults.PartSize),
                        LazyThreadSafetyMode.ExecutionAndPublication))
        ];
    }

    public DataMap Map { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMapFilePart Part(ref DataOffset offset)
    {
        _cleanLock.EnterReadLock();
        try
        {
            return Parts[offset.PartIndex].Value;
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
            for (var partId = 0; partId < DataDefaults.PartCount; partId++)
            {
                var part = Parts[partId];
                if (!part.IsValueCreated)
                {
                    continue;
                }

                part.Value.Flush();
            }

            FlushCore();
        }
        finally
        {
            _cleanLock.ExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void FlushCore()
    {
        RandomAccess.FlushToDisk(_safeFileHandle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (var dataPart in Parts.Where(x => x.IsValueCreated))
        {
            if (dataPart.Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        RandomAccess.FlushToDisk(_safeFileHandle);

        _safeFileHandle.Dispose();
        _disposed = true;
    }
}