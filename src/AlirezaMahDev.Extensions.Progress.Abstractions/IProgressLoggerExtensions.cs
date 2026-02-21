using System.Runtime.CompilerServices;

namespace AlirezaMahDev.Extensions.Progress.Abstractions;

public static class IProgressLoggerExtensions
{
    extension(IProgressLogger progressLogger)
    {
        public Task AutoReportAsync(Action func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync((_, _) =>
                {
                    func();
                    return ValueTask.CompletedTask;
                },
                cancellationToken);

        public Task AutoReportAsync(Action<IProgressLogger> func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync((progressLogger, _) =>
                {
                    func(progressLogger);
                    return ValueTask.CompletedTask;
                },
                cancellationToken);

        public Task AutoReportAsync(Action<IProgressLogger, CancellationToken> func,
            CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync((progressLogger, token) =>
                {
                    func(progressLogger, token);
                    return ValueTask.CompletedTask;
                },
                cancellationToken);

        [OverloadResolutionPriority(1)]
        public Task AutoReportAsync(Func<Task> func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync(async ValueTask (_, _) => await func(), cancellationToken);

        [OverloadResolutionPriority(1)]
        public Task AutoReportAsync(Func<CancellationToken, Task> func,
            CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync(async ValueTask (_, token) => await func(token),
                cancellationToken);

        [OverloadResolutionPriority(1)]
        public Task AutoReportAsync(Func<IProgressLogger, CancellationToken, Task> func,
            CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync(async (progressLogger, token) =>
                    await func(progressLogger, token),
                cancellationToken);

        public Task AutoReportAsync(Func<ValueTask> func, CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync(async (_, _) => await func(), cancellationToken);

        public Task AutoReportAsync(Func<CancellationToken, ValueTask> func,
            CancellationToken cancellationToken = default) =>
            progressLogger.AutoReportAsync(async ValueTask (_, token) => await func(token),
                cancellationToken);
    }
}