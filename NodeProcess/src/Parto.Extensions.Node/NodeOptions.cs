using System.Reflection;

namespace Parto.Extensions.Node;

public abstract class NodeOptions
{
    public string Path { get; set; } = "index.js";
    public Assembly Assembly { get; set; } = null!;
}