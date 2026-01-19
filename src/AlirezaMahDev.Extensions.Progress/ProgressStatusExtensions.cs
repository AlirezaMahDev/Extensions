using Microsoft.Extensions.Logging;

namespace AlirezaMahDev.Extensions.Progress;

public static class ProgressStatusExtensions
{
    extension(ILogger logger)
    {
        public ProgressLogger AsProgressLogger()
        {
            return ProgressLogger.Create(logger);
        }
    }
}