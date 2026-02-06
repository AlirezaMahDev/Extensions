using System.Runtime.CompilerServices;

namespace AlirezaMahDev.Extensions.Progress.Abstractions;

public static class IProgressLoggerExtensions
{
    extension(IProgressLogger progressLogger)
    {
        public Task AutoReportAsync(Action func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync((_, _) => { func(); return ValueTask.CompletedTask; }, cancellationToken);
        public Task AutoReportAsync(Action<IProgressLogger> func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync((progressLogger, _) => { func(progressLogger); return ValueTask.CompletedTask; }, cancellationToken);

        public Task AutoReportAsync(Action<IProgressLogger, CancellationToken> func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync((progressLogger, cancellationToken) => { func(progressLogger, cancellationToken); return ValueTask.CompletedTask; }, cancellationToken);

        [OverloadResolutionPriority(1)]
        public Task AutoReportAsync(Func<Task> func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync(async ValueTask (_, _) => await func(), cancellationToken);
        [OverloadResolutionPriority(1)]
        public Task AutoReportAsync(Func<CancellationToken, Task> func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync(async ValueTask (_, cancellationToken) => await func(cancellationToken), cancellationToken);
        [OverloadResolutionPriority(1)]
        public Task AutoReportAsync(Func<IProgressLogger, CancellationToken, Task> func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync(async (progressLogger, cancellationToken) => await func(progressLogger, cancellationToken), cancellationToken);

        public Task AutoReportAsync(Func<ValueTask> func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync(async (_, _) => await func(), cancellationToken);
        public Task AutoReportAsync(Func<CancellationToken, ValueTask> func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync(async ValueTask (_, cancellationToken) => await func(cancellationToken), cancellationToken);
    }

}
