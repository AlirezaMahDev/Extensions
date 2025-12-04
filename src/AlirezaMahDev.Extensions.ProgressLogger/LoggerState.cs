using System.Text.Json.Serialization;

namespace AlirezaMahDev.Extensions.ProgressLogger;

[method: JsonConstructor]
public record LoggerState(string Title, string Message)
{
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}