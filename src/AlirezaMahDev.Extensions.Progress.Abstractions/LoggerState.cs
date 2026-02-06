using System.Text.Json.Serialization;

namespace AlirezaMahDev.Extensions.Progress.Abstractions;

[method: JsonConstructor]
public record LoggerState(string Title, string Message)
{
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}