using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace AlirezaMahDev.Extensions.Node;

public abstract class NodeService
    : INodeService
{
    public Channel<NodeTaskRequest> Channel { get; } =
        System.Threading.Channels.Channel.CreateUnbounded<NodeTaskRequest>();

    public virtual ValueTask<JsonObject> InitializeAsync(CancellationToken cancellationToken = default)
    {
        var jsonObject = new JsonObject
        {
            { "path", Environment.CurrentDirectory }
        };
        return ValueTask.FromResult(jsonObject);
    }

    public virtual async Task InvokeVoidAsync(string name, CancellationToken cancellationToken = default)
    {
        await InvokeAsync(name, null, cancellationToken);
    }

    public virtual async Task InvokeVoidAsync<TParameter>(string name,
        TParameter? parameter,
        CancellationToken cancellationToken = default)
    {
        await InvokeAsync(name,
            parameter is null ? null : JsonSerializer.SerializeToElement(parameter, NodeDefaults.JsonSerializerOptions),
            cancellationToken);
    }

    public virtual async Task<TResult?> InvokeAsync<TResult>(string name,
        CancellationToken cancellationToken = default)
    {
        var jsonElement = await InvokeAsync(name, null, cancellationToken);
        return jsonElement.HasValue
            ? jsonElement.Value.Deserialize<TResult>(NodeDefaults.JsonSerializerOptions)
            : default;
    }

    public virtual async Task<TResult?> InvokeAsync<TResult, TParameter>(string name,
        TParameter? parameter,
        CancellationToken cancellationToken = default)
    {
        var jsonElement = await InvokeAsync(name,
            parameter is null ? null : JsonSerializer.SerializeToElement(parameter, NodeDefaults.JsonSerializerOptions),
            cancellationToken);
        return jsonElement.HasValue
            ? jsonElement.Value.Deserialize<TResult>(NodeDefaults.JsonSerializerOptions)
            : default;
    }

    public virtual async Task<JsonElement?> InvokeAsync(string name,
        JsonElement? parameter,
        CancellationToken cancellationToken = default)
    {
        NodeTaskRequest nodeTaskRequest = new(name, parameter);
        await Channel.Writer.WriteAsync(nodeTaskRequest, cancellationToken);
        var nodeTaskResponse = await nodeTaskRequest.TaskCompletionSource.Task;
        return nodeTaskResponse.Success ? nodeTaskResponse.Output : throw new(nodeTaskResponse.Error?.ToString());
    }
}

public abstract class NodeService<TConfig> : NodeService
{
    public override async ValueTask<JsonObject> InitializeAsync(CancellationToken cancellationToken = default)
    {
        var jsonObject = await base.InitializeAsync(cancellationToken);
        var config = await InitializeConfigAsync(cancellationToken);
        if (config is not null)
        {
            var configNode = JsonSerializer.SerializeToNode(config, NodeDefaults.JsonSerializerOptions);
            if (configNode is JsonObject configJsonObject)
            {
                foreach (var item in configJsonObject)
                {
                    jsonObject.Add(item.Key, item.Value);
                }
            }
        }

        return jsonObject;
    }

    public virtual ValueTask<TConfig?> InitializeConfigAsync(CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<TConfig?>(default);
    }
}