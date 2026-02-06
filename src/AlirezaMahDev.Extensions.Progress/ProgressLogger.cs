using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.Progress.Abstractions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlirezaMahDev.Extensions.Progress;

partial class ProgressLogger<T>(ILogger<T> logger, IOptionsMonitor<ProgressLoggerOptions> optionsMonitor)
    : ProgressLogger(logger, optionsMonitor), IProgressLogger<T>
{

}

partial class ProgressLogger(ILogger logger, IOptionsMonitor<ProgressLoggerOptions> optionsMonitor)
    : IDisposable, IProgressLogger
{
    [LoggerMessage(LogLevel.Information, "{message}")]
    private static partial void LogInformation(ILogger logger, string message);
    private readonly ProgressLoggerOptions _options = optionsMonitor.CurrentValue;

    public ProgressLoggerState State => _options.State;

    public Disposable Listener(EventHandler<ProgressLoggerState> action)
    {
        _options.Progress.ProgressChanged += action;
        return new(() => _options.Progress.ProgressChanged -= action);
    }

    public void Dispose()
    {
        ReportStop();
    }

    public void Report(int? count = null, int? length = null)
    {
        if (count.HasValue)
        {
            _options.Count = count.Value;
        }

        if (length.HasValue)
        {
            _options.Length = length.Value;
        }

        ProgressLoggerState value = new(_options.Name, _options.Message, _options.Count, _options.Length);
        LogInformation(logger, value.ToString());
        _options.ProgressInterface.Report(value);
    }

    public void Report(string message, int? count = null, int? length = null)
    {
        _options.Message = message;
        Report(count, length);
    }

    public void Report(string name, string message, int? count = null, int? length = null)
    {
        _options.Name = name;
        Report(message, count, length);
    }

    public void ReportStart(string name, string message = "start")
    {
        _options.Name = name;
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
        _options.Name = name;
    }

    public void SetMessage(string message)
    {
        _options.Message = message;
    }

    public void SetCount(int count)
    {
        _options.Count = count;
    }

    public void SetLength(int length)
    {
        _options.Length = length;
    }

    public void AddCount(int count)
    {
        Interlocked.Add(ref _options.RefCount, count);
    }

    public void IncrementCount()
    {
        Interlocked.Increment(ref _options.RefCount);
    }

    public void DecrementCount()
    {
        Interlocked.Decrement(ref _options.RefCount);
    }

    public async Task AutoReportAsync(
        Func<IProgressLogger, CancellationToken, ValueTask> func,
        CancellationToken cancellationToken = default)
    {
        var task = Task.Run(async () => await func(this, cancellationToken), cancellationToken);
        await Task.Yield();
        while (!task.IsCompleted)
        {
            Report();
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        await task.WaitAsync(cancellationToken);
    }
}