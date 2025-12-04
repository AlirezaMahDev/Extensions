using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Node;

public static class NodeExtensions
{
    extension(IExtensionsBuilder builder)
    {
        public NodeBuilder AddNode()
        {
            return new(builder);
        }

        public IExtensionsBuilder AddNode(Action<NodeBuilder> action)
        {
            action(AddNode(builder));
            return builder;
        }
    }
}