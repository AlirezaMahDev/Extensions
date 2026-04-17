using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace AlirezaMahDev.Extensions.Node;

public interface INodeService
{
    Channel<NodeTaskRequest> Channel { get; }

    ValueTask<JsonObject> InitializeAsync(CancellationToken cancellationToken = default);

    Task InvokeVoidAsync(string name, CancellationToken cancellationToken = default);

    Task InvokeVoidAsync<TParameter>(string name,
        TParameter? parameter,
        CancellationToken cancellationToken = default);

    Task<TResult?> InvokeAsync<TResult>(string name, CancellationToken cancellationToken = default);

    Task<TResult?> InvokeAsync<TResult, TParameter>(string name,
        TParameter? parameter,
        CancellationToken cancellationToken = default);

    Task<JsonElement?> InvokeAsync(string name,
        JsonElement? parameter,
        CancellationToken cancellationToken = default);
}