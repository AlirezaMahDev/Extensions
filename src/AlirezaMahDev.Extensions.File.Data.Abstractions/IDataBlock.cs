using AlirezaMahDev.Extensions.File.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataBlock
{
    IFileAccess FileAccess { get; }
    IDataBlockAccessor Read(DataAddress address);
}