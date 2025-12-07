using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.DataManager;

public static class DataManagerExtensions
{
    extension(IServiceCollection services)
    {
        public DataManagerBuilder AddDataManager() => new(services);

        public IServiceCollection AddDataManager(Action<DataManagerBuilder> action)
        {
            action(services.AddDataManager());
            return services;
        }
    }
}