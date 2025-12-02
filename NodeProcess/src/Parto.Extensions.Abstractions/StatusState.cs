using System.Text.Json.Serialization;

namespace Parto.Extensions.Abstractions;

[method: JsonConstructor]
public record StatusState(string Title, string Message)
{
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}