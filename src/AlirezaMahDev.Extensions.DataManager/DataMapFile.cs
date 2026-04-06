using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Win32.SafeHandles;

namespace AlirezaMahDev.Extensions.DataManager;

internal sealed class DataMapFile : IDisposable
{
    private bool _disposed;

    internal readonly Memory<Lazy<DataMapFilePart>> Parts;
    public SafeFileHandle SafeFileHandle { get; }
    private readonly ReaderWriterLockSlim _flushLock = new();


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMapFile(DataMap map, SafeFileHandle safeFileHandle)
    {
        Map = map;
        SafeFileHandle = safeFileHandle;
        Parts =
            new([
                .. Enumerable.Range(0, DataDefaults.PartCount)
                    .Select(index => new Lazy<DataMapFilePart>(() =>
                            new(this, index * DataDefaults.PartSize),
                        LazyThreadSafetyMode.ExecutionAndPublication))
            ]);
    }

    public DataMap Map { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMapFilePart Part(in DataOffset offset)
    {
        _flushLock.EnterReadLock();
        try
        {
            return Parts.Span[offset.PartIndex].Value;
        }
        finally
        {
            _flushLock.ExitReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Clean(Func<bool> force)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _flushLock.EnterReadLock();
        try
        {
            var result = false;
            for (var partId = 0; partId < DataDefaults.PartCount; partId++)
            {
                var part = Parts.Span[partId];
                if (!part.IsValueCreated)
                {
                    continue;
                }

                result |= part.Value.Clean(force());
            }

            return result;
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
            for (var partId = 0; partId < DataDefaults.PartCount; partId++)
            {
                var part = Parts.Span[partId];
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
            _flushLock.ExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void FlushCore()
    {
        RandomAccess.FlushToDisk(SafeFileHandle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _disposed = true;
        _flushLock.Dispose();

        for (int i = 0; i < Parts.Length; i++)
        {
            var part = Parts.Span[i];
            if (!part.IsValueCreated)
            {
                continue;
            }

            part.Value.Dispose();
        }

        FlushCore();
        SafeFileHandle.Dispose();
    }
}