using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager;
using AlirezaMahDev.Extensions.ParameterInstance;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlirezaMahDev.Extensions.Brain;

public class BrainBuilder
{
    public BrainBuilder(IServiceCollection services)
    {
        services.AddDataManager();
        services.TryAddSingleton<IBrainService, BrainService>();
        services.AddParameterInstanceFactory(typeof(NerveFactory<>));
    }
}