using System.Collections.Concurrent;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataLock : IDisposable
{
    private readonly ConcurrentDictionary<long, DataLockOffset> _locks = new();
    private bool _disposedValue;

    public DataLockOffset GetOrAdd(long offset)
    {
        return _locks.GetOrAdd(offset,
            static (innerOffset, arg) =>
                new(arg, innerOffset),
            this);
    }

    public void Remove(long offset)
    {
        if (_locks.TryRemove(offset, out var dataLockOffset))
        {
            dataLockOffset.Dispose();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
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