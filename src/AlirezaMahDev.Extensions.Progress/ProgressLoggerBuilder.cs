using AlirezaMahDev.Extensions.Progress.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlirezaMahDev.Extensions.Progress;

public class ProgressLoggerBuilder : BuilderBase
{
    public ProgressLoggerBuilder(IServiceCollection services) : base(services)
    {
        services.TryAddSingleton(typeof(IProgressLogger<>), typeof(ProgressLogger<>));
    }
}
