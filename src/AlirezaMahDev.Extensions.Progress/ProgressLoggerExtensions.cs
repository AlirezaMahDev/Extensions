using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.Progress;
public static class ProgressLoggerExtensions
{
    extension(IServiceCollection services)
    {
        public ProgressLoggerBuilder AddProgressLogger()
        {
            return new(services);
        }

        public IServiceCollection AddProgressLogger(Action<ProgressLoggerBuilder> action)
        {
            action(services.AddProgressLogger());
            return services;
        }
    }
}