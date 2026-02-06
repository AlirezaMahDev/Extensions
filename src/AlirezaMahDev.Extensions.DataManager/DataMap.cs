using System.IO.MemoryMappedFiles;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

sealed class DataMap(string path) : IDisposable
{
    private bool _disposedValue;

    private readonly Lazy<DataMapFile>[] _files =
    [
        .. Enumerable.Range(0, DataDefaults.FileCount)
            .Select(id =>
                new Lazy<DataMapFile>(() =>
                    {
                        var directoryPath = Path.GetDirectoryName(path)!;
                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);
                        return new(MemoryMappedFile.CreateFromFile(
                            string.Format(path, id),
                            FileMode.OpenOrCreate,
                            null,
                            DataDefaults.FileSize,
                            MemoryMappedFileAccess.ReadWrite)
                        );
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication))
    ];

    public DataMapFile File(in DataOffset offset)
    {
        return _files[offset.FileId].Value;
    }

    public void Flush()
    {
        foreach (var dataFile in _files.Where(x => x.IsValueCreated))
        {
            dataFile.Value.Flush();
        }
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        foreach (var dataFile in _files.Where(x => x.IsValueCreated))
        {
            await dataFile.Value.FlushAsync(cancellationToken);
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (var dataFile in _files.Where(x => x.IsValueCreated))
                {
                    dataFile.Value.Dispose();
                }
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
    }
}