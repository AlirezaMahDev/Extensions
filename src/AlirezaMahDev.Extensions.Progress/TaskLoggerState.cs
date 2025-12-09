using System.Text.Json.Serialization;

namespace AlirezaMahDev.Extensions.Progress;

[method: JsonConstructor]
public record TaskLoggerState(bool Success, string Title, string Message) : LoggerState(Title, Message);
