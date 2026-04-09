using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
internal class DataMapFilePart(DataMapFile file, int filePartOffset) : IDisposable
{
    public DataMapFile File { get; } = file;
    public int FilePartOffset { get; } = filePartOffset;

    internal ulong AliveCount;

    private readonly ReaderWriterLockSlim _flushLock = new(LockRecursionPolicy.SupportsRecursion);
    private DataMapFilePartCache? _cache;
    private bool _disposed;

    public bool HasCache
    {
        [MemberNotNullWhen(true, nameof(_cache))]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _cache is { IsInvalid: false };
        }
    }

    public bool HasChange
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return HasCache && _cache.HasChange;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMapFilePartCacheAccess GetCache(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_flushLock.TryEnterReadLock(0))
            {
                if (HasCache)
                {
                    return new(_cache, _flushLock);
                }

                _flushLock.ExitReadLock();
            }

            _flushLock.EnterUpgradeableReadLock();
            try
            {
                if (HasCache)
                {
                    continue;
                }

                _flushLock.EnterWriteLock();
                try
                {
                    _cache = new(this);
                }
                finally
                {
                    _flushLock.ExitWriteLock();
                }
            }
            finally
            {
                _flushLock.ExitUpgradeableReadLock();
            }
        }

        throw new OperationCanceledException(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public IDataAlive GetAlive()
    {
        return new DataAlive(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Clean(bool force)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_flushLock.IsWriteLockHeld || _flushLock.IsUpgradeableReadLockHeld)
        {
            return false;
        }

        if (!force && Interlocked.Read(ref AliveCount) != 0)
        {
            return false;
        }

        if (!_flushLock.TryEnterReadLock(0))
        {
            return false;
        }

        try
        {
            if (!HasCache)
            {
                return false;
            }
        }
        finally
        {
            _flushLock.ExitReadLock();
        }

        _flushLock.EnterUpgradeableReadLock();
        try
        {
            if (!HasCache)
            {
                return false;
            }

            _flushLock.EnterWriteLock();
            try
            {
                FlushCore();
                return true;
            }
            finally
            {
                _flushLock.ExitWriteLock();
            }
        }
        finally
        {
            _flushLock.ExitUpgradeableReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Flush()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _flushLock.EnterUpgradeableReadLock();
        try
        {
            if (!HasCache)
            {
                return;
            }

            _flushLock.EnterWriteLock();
            try
            {
                FlushCore();
            }
            finally
            {
                _flushLock.ExitWriteLock();
            }
        }
        finally
        {
            _flushLock.ExitUpgradeableReadLock();
        }
    }

    private void FlushCore()
    {
        if (!HasCache)
        {
            return;
        }

        _cache.Dispose();
        if (!_cache.IsInvalid)
            throw new("exist some enter access without exit access");
        _cache = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _disposed = true;
        _flushLock.Dispose();
        FlushCore();
    }
}