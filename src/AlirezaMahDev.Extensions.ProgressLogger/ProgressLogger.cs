using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace AlirezaMahDev.Extensions.ProgressLogger;

public static class ProgressStatusExtensions
{
    extension(ILogger logger)
    {
        public ProgressLogger AsProgressLogger(Action<ProgressLoggerState>? action = null, int length = -1)
        {
            return new(logger, action, length);
        }
    }
}

public partial class ProgressLogger(ILogger? logger, Action<ProgressLoggerState>? action = null, int length = -1)
    : Progress<ProgressLoggerState>(action ?? (_ => { })),IDisposable
{
    private int _count;
    private Stopwatch _stopwatch = Stopwatch.StartNew();

    public ProgressLoggerState State { get; private set; } = ProgressLoggerState.Empty;

    [LoggerMessage(LogLevel.Information, "{message}")]
    private static partial void LogInformation(ILogger logger, string message);

    private string Name
    {
        get;
        set => Interlocked.Exchange(ref field, value);
    } = string.Empty;

    private string Message
    {
        get;
        set => Interlocked.Exchange(ref field, value);
    } = string.Empty;

    private int Count
    {
        get => _count;
        set => Interlocked.Exchange(ref _count, value);
    }

    private int Length
    {
        get;
        set => Interlocked.Exchange(ref field, value);
    } = length;

    protected override void OnReport(ProgressLoggerState value)
    {
        if (logger != null)
        {
            LogInformation(logger, value.ToString());
        }

        State = value;
        base.OnReport(value);
    }

    public void Report(int? count = null, int? length = null)
    {
        if (count.HasValue)
        {
            Count = count.Value;
        }

        if (length.HasValue)
        {
            Length = length.Value;
        }

        OnReport(new(Name, Message, Count, Length));
    }

    public void Report(string message, int? count = null, int? length = null)
    {
        Message = message;
        Report(count, length);
    }

    public void Report(string name, string message, int? count = null, int? length = null)
    {
        Name = name;
        Report(message, count, length);
    }

    public void ReportStart(string name, string message = "start")
    {
        Name = name;
        Report(message, 0, -1);
    }

    public void ReportProgress(string message, int length)
    {
        Report(message, 0, length);
    }

    public void ReportIndeterminate(string message)
    {
        Report(message, 0, -1);
    }

    public void ReportStop(string message = "stop")
    {
        Report(message, 0, 0);
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetMessage(string message)
    {
        Message = message;
    }

    public void SetCount(int count)
    {
        Count = count;
    }

    public void SetLength(int length)
    {
        Length = length;
    }

    public void AddCount(int count)
    {
        Interlocked.Add(ref _count, count);
    }

    public void IncrementCount()
    {
        Interlocked.Increment(ref _count);
    }

    public void DecrementCount()
    {
        Interlocked.Decrement(ref _count);
    }

    public async Task AutoReportAsync(
        Func<CancellationToken, Task> func,
        CancellationToken cancellationToken = default)
    {
        var task = Task.Run(async () => await func(cancellationToken), cancellationToken);
        while (!task.IsCompleted)
        {
            Report();
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        await task.WaitAsync(cancellationToken);
    }

    public void Dispose()
    {
        ReportStop();
    }
}