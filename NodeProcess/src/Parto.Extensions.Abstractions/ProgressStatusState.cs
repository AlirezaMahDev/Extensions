using System.Text.Json.Serialization;

namespace Parto.Extensions.Abstractions;

[method: JsonConstructor]
public record ProgressStatusState(string Title, string Message, int Count, int Length)
    : StatusState(Title, Message)
{
    public static ProgressStatusState Empty { get; } = new(string.Empty, string.Empty, 0, 0);

    [JsonIgnore]
    public bool IsIndeterminate => Length == -1;

    [JsonIgnore]
    public double ProgressValue => IsIndeterminate ? 0 : (double)Count / Length;

    public override string ToString()
    {
        return $"Timestamp{Timestamp.LocalDateTime} Name:{Title} Message:{Message} Count:{Count} Length:{Length
        } Progress:{ProgressValue}";
    }
}

[method: JsonConstructor]
public record ProgressStatusState<TState>(string Title, TState State, string Message, int Count, int Length)
    : ProgressStatusState(Title, Message, Count, Length);