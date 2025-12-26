using System.Text.Json;
using System.Threading.Channels;

namespace AlirezaMahDev.Extensions.Node;

public interface INodeService
{
    Channel<NodeTaskRequest> Channel { get; }

    ValueTask<JsonElement?> InitializeAsync(CancellationToken cancellationToken = default);

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