using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AlirezaMahDev.Extensions.DataManager;

public class DataManagerOptions
{
    public string DirectoryPath { get; set; } = Path.Combine(Environment.CurrentDirectory, ".data");
    public string FileFormat { get; set; } = "{0}.db";
    public string DefaultName { get; set; } = "data";
}

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
                .Open(Path.Combine(options.DirectoryPath,
                    string.Format(options.FileFormat, options.DefaultName)));
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
                    .Open(Path.Combine(options.DirectoryPath,
                        string.Format(options.FileFormat, key)));
            });
        return this;
    }

    public DataManagerBuilder AddAccess(string key, string path)
    {
        Services.TryAddKeyedSingleton<IDataAccess>(key,
            (provider, _) => provider.GetRequiredService<IDataManager>().Open(path));
        return this;
    }
}

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

public interface IDataManager
{
    IDataAccess Open(string path);
    bool Close(string path);
}

class DataManager : IDisposable, IDataManager
{
    private readonly ConcurrentDictionary<string, Lazy<DataAccess>> _cache = [];
    private bool _disposedValue;

    public IDataAccess Open(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return _cache.GetOrAdd(path,
                static (key, arg) =>
                    new(() => new(key), LazyThreadSafetyMode.ExecutionAndPublication),
                this)
            .Value;
    }

    public bool Close(string path)
    {
        if (!_cache.TryRemove(path, out var dataAccess))
        {
            return false;
        }

        if (!dataAccess.IsValueCreated)
        {
            return true;
        }

        dataAccess.Value.Dispose();
        return true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (var dataAccess in _cache.Values)
                {
                    if (dataAccess.IsValueCreated)
                    {
                        dataAccess.Value.Dispose();
                    }
                }

                _cache.Clear();
            }

            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}