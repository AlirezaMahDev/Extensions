using AlirezaMahDev.Extensions.Progress.Abstractions;

namespace AlirezaMahDev.Extensions.Progress;

internal class ProgressLoggerOptions
{
    public string Name
    {
        get => Volatile.Read(ref field);
        set => Volatile.Write(ref field, value);
    } = string.Empty;

    public string Message
    {
        get => Volatile.Read(ref field);
        set => Volatile.Write(ref field, value);
    } = string.Empty;

    private int _count;
    public ref int RefCount => ref _count;

    public int Count
    {
        get => Volatile.Read(ref _count);
        set => Volatile.Write(ref _count, value);
    }

    private int _length;
    public ref int RefLength => ref _length;

    public int Length
    {
        get => Volatile.Read(ref _length);
        set => Volatile.Write(ref _length, value);
    }

    public Progress<ProgressLoggerState> Progress { get; } = new();
    public IProgress<ProgressLoggerState> ProgressInterface => Progress;
    public ProgressLoggerState State => new(Name, Message, Count, Length, LastState);

    public ProgressLoggerState? LastState
    {
        get => Volatile.Read(ref field);
        set => Volatile.Write(ref field, value);
    }

    public ProgressLoggerState GenerateState()
    {
        var result = State;
        LastState = result;
        return result;
    }
}