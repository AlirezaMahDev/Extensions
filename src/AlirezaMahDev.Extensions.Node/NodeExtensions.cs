using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.Node;

public static class NodeExtensions
{
    extension(IServiceCollection services)
    {
        public NodeBuilder AddNode()
        {
            return new(services);
        }

        public IServiceCollection AddNode(Action<NodeBuilder> action)
        {
            action(AddNode(services));
            return services;
        }
    }
}