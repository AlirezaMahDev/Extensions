using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IExtensionsBuilder
{
    IExtensionsEnvironment Environment { get; }
    IConfiguration Configuration { get; }
    IServiceCollection Services { get; }
}