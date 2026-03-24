using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AlirezaMahDev.Extensions.DataManager;

public class DataManagerBuilder : BuilderBase
{
    public DataManagerBuilder(IServiceCollection services) : base(services)
    {
        services.TryAddSingleton<IDataManager, DataManager>();
    }

    public DataManagerBuilder UseDefault()
    {
        Services.TryAddSingleton<IDataAccess>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<DataManagerOptions>>().Value;
            return provider.GetRequiredService<IDataManager>()
                .Open(Path.Combine(options.DirectoryPath, options.DefaultName));
        });
        return this;
    }

    public DataManagerBuilder AddAccess(string key)
    {
        Services.TryAddKeyedSingleton<IDataAccess>(key,
            (provider, _) =>
            {
                var options = provider.GetRequiredService<IOptions<DataManagerOptions>>().Value;
                return provider.GetRequiredService<IDataManager>()
                    .Open(Path.Combine(options.DirectoryPath, key));
            });
        return this;
    }
}