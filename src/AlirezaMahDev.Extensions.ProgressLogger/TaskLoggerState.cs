using System.Text.Json.Serialization;

namespace AlirezaMahDev.Extensions.ProgressLogger;

[method: JsonConstructor]
public record TaskLoggerState(bool Success, string Title, string Message) : LoggerState(Title, Message);
