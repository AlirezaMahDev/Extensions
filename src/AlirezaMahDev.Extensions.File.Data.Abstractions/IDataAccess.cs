using AlirezaMahDev.Extensions.File.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataAccess
{
    IFileAccess FileAccess { get; }
    IDataLocation Root { get; }
}