using System.Text.Json.Serialization;

namespace AlirezaMahDev.Extensions.Progress.Abstractions;

[method: JsonConstructor]
public record ProgressLoggerState(
    string Title,
    string Message,
    int Count,
    int Length,
    [property: JsonIgnore]
    ProgressLoggerState? Last)
    : LoggerState(Title, Message)
{
    public static ProgressLoggerState Empty { get; } =
        new(string.Empty, string.Empty, 0, 0, null);

    [JsonIgnore]
    public bool IsIndeterminate => Length == -1;

    [JsonIgnore]
    public double ProgressValue => IsIndeterminate ? 0 : (double)Count / Length;

    [JsonIgnore]
    public double? ProgressSpeed =>
        Last is not null && (Timestamp - Last.Timestamp).TotalSeconds is var seconds && seconds != 0
            ? (ProgressValue - Last.ProgressValue) / seconds
            : null;

    public override string ToString() =>
        $"Name:{Title} Message:{Message} Count:{Count} Length:{
            Length} {ProgressValue:##,##} Speed:{ProgressSpeed:##,##}/s";
}

[method: JsonConstructor]
public record ProgressLoggerState<TState>(
    string Title,
    TState State,
    string Message,
    int Count,
    int Length,
    ProgressLoggerState? Last)
    : ProgressLoggerState(Title, Message, Count, Length, Last);