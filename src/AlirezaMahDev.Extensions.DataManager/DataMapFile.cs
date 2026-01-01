using System.Collections.Concurrent;
using System.IO.MemoryMappedFiles;

namespace AlirezaMahDev.Extensions.DataManager;

class DataMapFile(MemoryMappedFile file) : IDisposable
{
    private bool _disposedValue;
    private readonly ConcurrentDictionary<long, Lazy<DataMapFilePart>> _parts = [];

    public DataMapFilePart Part(long offset) =>
        _parts.GetOrAdd(DataHelper.PartIndex(offset), static (id, file) =>
                new(() =>
                    new(file.CreateViewAccessor(id, DataDefaults.PartSize, MemoryMappedFileAccess.ReadWrite)),
                LazyThreadSafetyMode.ExecutionAndPublication),
            file)
        .Value;

    public void Flush()
    {
        foreach (var dataPart in _parts.Values)
        {
            dataPart.Value.Flush();
        }
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        foreach (var dataPart in _parts.Values)
        {
            await dataPart.Value.FlushAsync(cancellationToken);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (var dataPart in _parts.Values)
                {
                    if (dataPart.Value is IDisposable disposable)
                        disposable.Dispose();
                }

                file.Dispose();
            }

            _parts.Clear();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
