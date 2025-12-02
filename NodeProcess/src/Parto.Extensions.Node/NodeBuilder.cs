using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Parto.Extensions.Abstractions;

namespace Parto.Extensions.Node;

public class NodeBuilder(IExtensionsBuilder extensionsBuilder)
{
    public NodeBuilder Add<TNodeService, TNodeServiceOptions>(Action<NodeOptions>? action = null)
        where TNodeService : class, INodeService
        where TNodeServiceOptions : NodeOptions
    {
        extensionsBuilder.Services.TryAddSingleton<TNodeService>();
        var optionsBuilder = extensionsBuilder.Services.AddOptions<TNodeServiceOptions>();
        optionsBuilder.Configure(options => options.Assembly = typeof(TNodeService).Assembly);
        if (action != null)
        {
            optionsBuilder.Configure(action);
        }

        extensionsBuilder.Services.AddHostedService<NodeWorker<TNodeService, TNodeServiceOptions>>();
        return this;
    }
}