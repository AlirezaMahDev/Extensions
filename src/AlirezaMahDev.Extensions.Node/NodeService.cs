using System.Text.Json;
using System.Threading.Channels;

namespace AlirezaMahDev.Extensions.Node;

public abstract class NodeService
    : INodeService
{
    public Channel<NodeTaskRequest> Channel { get; } =
        System.Threading.Channels.Channel.CreateUnbounded<NodeTaskRequest>();

    public virtual ValueTask<JsonElement?> InitializeAsync(CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<JsonElement?>(null);
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
    public override async ValueTask<JsonElement?> InitializeAsync(CancellationToken cancellationToken = default)
    {
        return JsonSerializer.SerializeToElement(await InitializeConfigAsync(cancellationToken),
            NodeDefaults.JsonSerializerOptions);
    }

    public virtual ValueTask<TConfig?> InitializeConfigAsync(CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<TConfig?>(default);
    }
}