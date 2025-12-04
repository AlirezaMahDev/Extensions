using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

using FileOptions = AlirezaMahDev.Extensions.File.Abstractions.FileOptions;

namespace AlirezaMahDev.Extensions.File;

internal class FileBuilder : IFileBuilder
{
    public FileBuilder(IExtensionsBuilder builder)
    {
        ExtensionsBuilder = builder;

        ExtensionsBuilder.Services.TryAddSingleton<IFileService, FileService>();
        ExtensionsBuilder.Services.AddParameterInstanceFactory<FileAccessFactory>();

        OptionsBuilder = ExtensionsBuilder.Services.AddOptions<FileOptions>();
        OptionsBuilder.PostConfigure(options =>
        {
            if (!Directory.Exists(options.Path))
            {
                Directory.CreateDirectory(options.Path);
            }
        });
    }

    public IExtensionsBuilder ExtensionsBuilder { get; }
    public OptionsBuilder<FileOptions> OptionsBuilder { get; }
}