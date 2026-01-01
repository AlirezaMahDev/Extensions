using System.Collections.Concurrent;
using System.IO.MemoryMappedFiles;

namespace AlirezaMahDev.Extensions.DataManager;

class DataMap(string path) : IDisposable
{
    private bool _disposedValue;
    private readonly ConcurrentDictionary<long, Lazy<DataMapFile>> _files = [];

    public DataMapFile File(long offset) =>
        _files.GetOrAdd(DataHelper.FileId(offset), static (id, path) =>
            new(() =>
                new(MemoryMappedFile.CreateFromFile(
                    string.Format(path, id),
                    FileMode.OpenOrCreate,
                    null,
                    DataDefaults.FileSize,
                    MemoryMappedFileAccess.ReadWrite)
                ), LazyThreadSafetyMode.ExecutionAndPublication)
           , path)
        .Value;

    public void Flush()
    {
        foreach (var dataFile in _files.Values)
        {
            dataFile.Value.Flush();
        }
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        foreach (var dataFile in _files.Values)
        {
            await dataFile.Value.FlushAsync(cancellationToken);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (var dataFile in _files.Values)
                {
                    dataFile.Value.Dispose();
                }
            }

            _files.Clear();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
