using System.Text.Json.Serialization;

namespace AlirezaMahDev.Extensions.Progress;

[method: JsonConstructor]
public record ProgressLoggerState(string Title, string Message, int Count, int Length)
    : LoggerState(Title, Message)
{
    public static ProgressLoggerState Empty { get; } = new(string.Empty, string.Empty, 0, 0);

    [JsonIgnore]
    public bool IsIndeterminate => Length == -1;

    [JsonIgnore]
    public double ProgressValue => IsIndeterminate ? 0 : (double)Count / Length;

    public override string ToString()
    {
        return $"Timestamp{Timestamp.LocalDateTime} Name:{Title} Message:{Message} Count:{Count} Length:{Length} Progress:{ProgressValue}";
    }
}

[method: JsonConstructor]
public record ProgressLoggerState<TState>(string Title, TState State, string Message, int Count, int Length)
    : ProgressLoggerState(Title, Message, Count, Length);