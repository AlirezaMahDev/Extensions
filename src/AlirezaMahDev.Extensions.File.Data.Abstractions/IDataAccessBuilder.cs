using AlirezaMahDev.Extensions.File.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataAccessBuilder
{
    IFileBuilder FileBuilder { get; }
}