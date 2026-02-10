using AlirezaMahDev.Extensions.Progress.Abstractions;

namespace AlirezaMahDev.Extensions.Progress;

class ProgressLoggerOptions
{
    public string Name
    {
        get => Volatile.Read(ref field);
        set => Interlocked.Exchange(ref field, value);
    } = string.Empty;

    public string Message
    {
        get => Volatile.Read(ref field);
        set => Interlocked.Exchange(ref field, value);
    } = string.Empty;

    private int _count;
    public ref int RefCount => ref _count;

    public int Count
    {
        get => Volatile.Read(ref _count);
        set => Interlocked.Exchange(ref _count, value);
    }

    public int Length
    {
        get => Volatile.Read(ref field);
        set => Interlocked.Exchange(ref field, value);
    } = 0;

    public Progress<ProgressLoggerState> Progress { get; } = new();
    public IProgress<ProgressLoggerState> ProgressInterface => Progress;
    public ProgressLoggerState State => new(Name, Message, Count, Length, LastState);

    public ProgressLoggerState? LastState
    {
        get => Volatile.Read(ref field);
        set => Interlocked.Exchange(ref field, value);
    }

    public ProgressLoggerState GenerateState()
    {
        var result = State;
        LastState = result;
        return result;
    }
}