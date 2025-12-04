using System.IO.MemoryMappedFiles;
using System.Numerics;

using Microsoft.Extensions.DependencyInjection;

using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data;

internal sealed class DataBlock : IDisposable, IDataBlock
{
    public DataAccess DataAccess { get; }
    private readonly DataBlockAccessorFactory _accessorFactory;
    private readonly Lock _lockFile = new();
    private readonly FileStream _fileStream;

    private MemoryMappedFile _memoryMappedFile = null!;

    public DataBlock(IServiceProvider provider, DataAccess dataAccess)
    {
        DataAccess = dataAccess;
        FileAccess = dataAccess.FileAccess;
        _fileStream = FileAccess.FileStream;
        _accessorFactory = ActivatorUtilities.CreateInstance<DataBlockAccessorFactory>(provider);
        _lockFile.Enter();
        SetLengthCore(uint.MaxValue);
    }

    public IFileAccess FileAccess { get; }

    public void Dispose()
    {
        _memoryMappedFile.Dispose();
    }

    private void SetLength(long length)
    {
        if (_fileStream.Length >= length)
        {
            return;
        }

        _lockFile.Enter();
        if (_fileStream.Length >= length)
        {
            return;
        }

        foreach (var block in _accessorFactory)
        {
            block.UnInitialize();
        }

        _memoryMappedFile.Dispose();
        SetLengthCore(length);
    }

    private void SetLengthCore(long length)
    {
        if (_fileStream.Length < length)
        {
            _fileStream.SetLength(length);
        }

        _memoryMappedFile = MemoryMappedFile.CreateFromFile(_fileStream,
            null,
            (long)BitOperations.RoundUpToPowerOf2((ulong)_fileStream.Length),
            MemoryMappedFileAccess.ReadWrite,
            HandleInheritability.Inheritable,
            true);
        foreach (var block in _accessorFactory)
        {
            block.Initialize();
        }

        _lockFile.Exit();
    }

    public MemoryMappedViewAccessor CreateViewAccessor(DataAddress address)
    {
        SetLength(address.Length + address.Id);
        return _memoryMappedFile.CreateViewAccessor(address.Id,
            address.Length,
            MemoryMappedFileAccess.ReadWrite);
    }

    public IDataBlockAccessor Read(DataAddress address)
    {
        return _accessorFactory.GetOrCreate(new(this, address));
    }
}