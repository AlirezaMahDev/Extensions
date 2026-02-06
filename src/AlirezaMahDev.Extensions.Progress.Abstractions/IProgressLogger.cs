using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Progress.Abstractions;

public interface IProgressLogger
{
    ProgressLoggerState State { get; }

    void AddCount(int count);
    Task AutoReportAsync(Func<IProgressLogger, CancellationToken, ValueTask> func, CancellationToken cancellationToken = default);
    void DecrementCount();
    void IncrementCount();
    void Report(int? count = null, int? length = null);
    void Report(string message, int? count = null, int? length = null);
    void Report(string name, string message, int? count = null, int? length = null);
    void ReportIndeterminate(string message);
    void ReportProgress(string message, int length);
    void ReportStart(string name, string message = "start");
    void ReportStop(string message = "stop");
    void SetCount(int count);
    void SetLength(int length);
    void SetMessage(string message);
    void SetName(string name);

    public Disposable Listener(EventHandler<ProgressLoggerState> action);
}

public interface IProgressLogger<T> : IProgressLogger;
