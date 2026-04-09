using AlirezaMahDev.Extensions.Progress.Abstractions;

namespace AlirezaMahDev.Extensions.Progress;

internal class ProgressLoggerOptions
{
    public string Name
    {
        get => Interlocked.Read(ref field);
        set => Interlocked.Exchange(ref field, value);
    } = string.Empty;

    public string Message
    {
        get => Interlocked.Read(ref field);
        set => Interlocked.Exchange(ref field, value);
    } = string.Empty;

    private int _count;
    public ref int RefCount => ref _count;

    public int Count
    {
        get => Interlocked.Read(ref _count);
        set => Interlocked.Exchange(ref _count, value);
    }

    private int _length;
    public ref int RefLength => ref _length;

    public int Length
    {
        get => Interlocked.Read(ref _length);
        set => Interlocked.Exchange(ref _length, value);
    }

    public Progress<ProgressLoggerState> Progress { get; } = new();
    public IProgress<ProgressLoggerState> ProgressInterface => Progress;
    public ProgressLoggerState State => new(Name, Message, Count, Length, LastState);

    public ProgressLoggerState? LastState
    {
        get => Interlocked.Read(ref field);
        set => Interlocked.Exchange(ref field, value);
    }

    public ProgressLoggerState GenerateState()
    {
        var result = State;
        LastState = result;
        return result;
    }
}