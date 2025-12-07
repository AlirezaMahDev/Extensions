using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using FileOptions = AlirezaMahDev.Extensions.File.Abstractions.FileOptions;

namespace AlirezaMahDev.Extensions.File;

internal class FileBuilder : BuilderBase<FileOptions>, IFileBuilder
{
    public FileBuilder(IServiceCollection services) : base(services)
    {
        services.TryAddSingleton<IFileService, FileService>();
        services.AddParameterInstanceFactory<FileAccessFactory>();

        OptionsBuilder.PostConfigure(options =>
        {
            if (!Directory.Exists(options.Path))
            {
                Directory.CreateDirectory(options.Path);
            }
        });
    }
}