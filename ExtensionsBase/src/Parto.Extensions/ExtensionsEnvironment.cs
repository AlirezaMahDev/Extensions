using Parto.Extensions.Abstractions;

namespace Parto.Extensions;

public class ExtensionsEnvironment(string environmentName) : IExtensionsEnvironment
{
    public string EnvironmentName { get; set; } = environmentName;
}