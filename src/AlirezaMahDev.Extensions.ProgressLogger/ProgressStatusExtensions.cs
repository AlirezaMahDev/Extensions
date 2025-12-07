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