using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions;

public class ExtensionsEnvironment(string environmentName) : IExtensionsEnvironment
{
    public string EnvironmentName { get; set; } = environmentName;
}