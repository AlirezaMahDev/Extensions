using System.Text.Json;
using System.Text.Json.Serialization;

namespace Parto.Extensions.Node;

[method: JsonConstructor]
public record NodeTaskResponse(Guid Id, string Name, bool Success, JsonElement? Output, JsonElement? Error);