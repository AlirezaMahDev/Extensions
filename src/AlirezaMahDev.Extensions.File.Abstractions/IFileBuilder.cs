using Microsoft.Extensions.Options;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.File.Abstractions;

public interface IFileBuilder
{
    IExtensionsBuilder ExtensionsBuilder { get; }
    OptionsBuilder<FileOptions> OptionsBuilder { get; }
}