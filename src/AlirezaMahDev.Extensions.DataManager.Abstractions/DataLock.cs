using System.Collections.Concurrent;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataLock : IDisposable
{
    private readonly ReaderWriterLockSlim _readerWriterLockSlim = new();
    private readonly ConcurrentDictionary<long, DataLockOffset> _locks = new();
    private bool _disposedValue;

    public DataLockOffset GetOrAdd(long offset)
    {
        _readerWriterLockSlim.EnterReadLock();
        try
        {
            return _locks.GetOrAdd(offset,
                static (innerOffset, arg) =>
                    new(arg, innerOffset),
                this);
        }
        finally
        {
            _readerWriterLockSlim.ExitReadLock();
        }
    }

    public void Remove(long offset)
    {
        if (_locks.TryGetValue(offset, out var dataLockOffset) && dataLockOffset.CurrentCount == 0)
        {
            _readerWriterLockSlim.EnterWriteLock();
            try
            {
                if (_locks.TryGetValue(offset, out dataLockOffset) && dataLockOffset.CurrentCount == 0)
                {
                    if (_locks.TryRemove(offset, out dataLockOffset))
                    {
                        dataLockOffset.Dispose();
                    }
                }
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _readerWriterLockSlim.Dispose();
                foreach (var dataLockOffset in _locks.Values)
                {
                    dataLockOffset.Dispose();
                }
            }

            _locks.Clear();
            _disposedValue = true;
        }
    }


    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}