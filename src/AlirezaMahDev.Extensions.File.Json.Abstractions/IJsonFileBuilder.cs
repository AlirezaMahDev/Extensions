using AlirezaMahDev.Extensions.File.Abstractions;

namespace AlirezaMahDev.Extensions.File.Json.Abstractions;

public interface IJsonFileBuilder
{
    IFileBuilder FileBuilder { get; }
}