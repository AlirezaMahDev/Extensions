using System.Reflection;

namespace AlirezaMahDev.Extensions.Node;

public abstract class NodeOptions
{
    public Assembly Assembly { get; set; } = null!;
}