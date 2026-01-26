using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.Options;

namespace AlirezaMahDev.Extensions.DataManager;

sealed class DataManager(IOptions<DataManagerOptions> options) : IDisposable, IDataManager
{
    private readonly IOptions<DataManagerOptions> _options = options;
    private readonly ConcurrentDictionary<string, Lazy<DataAccess>> _cache = [];
    private bool _disposedValue;

    public IDataAccess Open(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _cache.GetOrAdd(key,
                static (key, arg) => new(() =>
                    {
                        var optionsValue = arg._options.Value;
                        var path = Path.Combine(optionsValue.DirectoryPath, key);
                        return new(path);
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication),
                this)
            .Value;
    }

    public ITempDataAccess OpenTemp() => new TempDataAccess();

    public bool Close(string key)
    {
        if (!_cache.TryRemove(key, out var dataAccess))
        {
            return false;
        }

        if (!dataAccess.IsValueCreated)
        {
            return true;
        }

        dataAccess.Value.Dispose();
        return true;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (var dataAccess in _cache.Values)
                {
                    if (dataAccess.IsValueCreated)
                    {
                        dataAccess.Value.Dispose();
                    }
                }

                _cache.Clear();
            }

            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}