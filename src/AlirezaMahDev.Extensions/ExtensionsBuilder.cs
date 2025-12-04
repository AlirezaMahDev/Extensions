using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions;

class ExtensionsBuilder(
    string environmentName,
    IConfiguration configuration,
    IServiceCollection services) : BuilderBase<ExtensionsOptions>(services), IExtensionsBuilder
{
    public IExtensionsEnvironment Environment { get; } = new ExtensionsEnvironment(environmentName);
    public IConfiguration Configuration { get; } = configuration;
    public IServiceCollection Services { get; } = services;
}

// public static Lazy<T> LazyRequireService<T>()
//     where T : notnull => new(() => App.CurrentApp.Host.Services.GetRequiredService<T>(),
//     LazyThreadSafetyMode.ExecutionAndPublication);