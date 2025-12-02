using Microsoft.Extensions.Hosting;

using Parto.Extensions.Abstractions;

namespace Parto.Extensions;

public static class Extensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseExtensions(Action<IExtensionsBuilder> action)
        {
            builder.ConfigureServices((context, services) => action(new ExtensionsBuilder(
                context.HostingEnvironment.EnvironmentName,
                context.Configuration,
                services
            )));
            return builder;
        }
    }

    extension(IHostApplicationBuilder builder)
    {
        public IExtensionsBuilder UseExtensions()
        {
            return new ExtensionsBuilder(
                builder.Environment.EnvironmentName,
                builder.Configuration,
                builder.Services
            );
        }

        public IHostApplicationBuilder UseExtensions(Action<IExtensionsBuilder> action)
        {
            action(builder.UseExtensions());
            return builder;
        }
    }

    extension(IExtensionsEnvironment extensionsEnvironment)
    {
        public bool IsDevelopment()
        {
            return extensionsEnvironment.IsEnvironment(Environments.Development);
        }

        public bool IsStaging()
        {
            return extensionsEnvironment.IsEnvironment(Environments.Staging);
        }

        public bool IsProduction()
        {
            return extensionsEnvironment.IsEnvironment(Environments.Production);
        }

        public bool IsEnvironment(string environmentName)
        {
            return string.Equals(extensionsEnvironment.EnvironmentName,
                environmentName,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}