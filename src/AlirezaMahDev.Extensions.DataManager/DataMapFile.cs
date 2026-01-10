using System.IO.MemoryMappedFiles;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

class DataMapFile(MemoryMappedFile file) : IDisposable
{
    private bool _disposedValue;

    private readonly Lazy<DataMapFilePart>[] _parts =
    [
        .. Enumerable.Range(0, DataDefaults.PartCount)
            .Select(index => new Lazy<DataMapFilePart>(() =>
                    new(file.CreateViewAccessor(index * DataDefaults.PartSize,
                        DataDefaults.PartSize,
                        MemoryMappedFileAccess.ReadWrite)),
                LazyThreadSafetyMode.ExecutionAndPublication))
    ];

    public DataMapFilePart Part(in DataOffset offset)
    {
        return _parts[offset.PartIndex].Value;
    }

    public void Flush()
    {
        foreach (var dataPart in _parts.Where(x => x.IsValueCreated))
        {
            dataPart.Value.Flush();
        }
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        foreach (var dataPart in _parts.Where(x => x.IsValueCreated))
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
                foreach (var dataPart in _parts.Where(x => x.IsValueCreated))
                {
                    if (dataPart.Value is IDisposable disposable)
                        disposable.Dispose();
                }

                file.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}