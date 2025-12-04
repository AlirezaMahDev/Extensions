using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlirezaMahDev.Extensions.Node;

public class NodeBuilder(IServiceCollection services) : BuilderBase(services)
{
    public NodeBuilder Add<TNodeService, TNodeServiceOptions>(Action<NodeOptions>? action = null)
        where TNodeService : class, INodeService
        where TNodeServiceOptions : NodeOptions
    {
        Services.TryAddSingleton<TNodeService>();
        var optionsBuilder = Services.AddOptions<TNodeServiceOptions>();
        optionsBuilder.Configure(options => options.Assembly = typeof(TNodeService).Assembly);
        if (action != null)
        {
            optionsBuilder.Configure(action);
        }

        Services.AddHostedService<NodeWorker<TNodeService, TNodeServiceOptions>>();
        return this;
    }
}