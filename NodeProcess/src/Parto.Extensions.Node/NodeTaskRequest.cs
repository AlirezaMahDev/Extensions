using System.Text.Json;
using System.Text.Json.Serialization;

namespace Parto.Extensions.Node;

[method: JsonConstructor]
public record NodeTaskRequest(string Name, JsonElement? Input)
{
    public Guid Id { get; } = Guid.CreateVersion7();

    [JsonIgnore]
    public TaskCompletionSource<NodeTaskResponse> TaskCompletionSource { get; } = new();
}