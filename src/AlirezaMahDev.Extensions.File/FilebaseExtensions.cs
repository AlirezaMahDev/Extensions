using AlirezaMahDev.Extensions.File.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.File;

public static class FilebaseExtensions
{
    extension(IServiceCollection services)
    {
        public IFileBuilder AddFileService()
        {
            return new FileBuilder(services);
        }

        public IServiceCollection AddFileService(Action<IFileBuilder> action)
        {
            action(services.AddFileService());
            return services;
        }
    }
}