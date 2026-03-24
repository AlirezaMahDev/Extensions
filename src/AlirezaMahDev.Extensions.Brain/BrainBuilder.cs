namespace AlirezaMahDev.Extensions.Brain;

public class BrainBuilder
{
    public BrainBuilder(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddDataManager();
        services.TryAddSingleton<IBrainService, BrainService>();
        services.AddSingletonParameterInstanceFactory(typeof(NerveFactory<,>));
    }
}