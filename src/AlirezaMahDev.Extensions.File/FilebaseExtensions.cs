using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.File.Abstractions;

namespace AlirezaMahDev.Extensions.File;

public static class FilebaseExtensions
{
    extension(IExtensionsBuilder builder)
    {
        public IFileBuilder AddFileService()
        {
            return new FileBuilder(builder);
        }

        public IExtensionsBuilder AddFileService(Action<IFileBuilder> action)
        {
            action(AddFileService(builder));
            return builder;
        }
    }
}