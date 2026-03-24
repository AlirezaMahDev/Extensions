using System.Collections;
using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.ParameterInstance.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AlirezaMahDev.Extensions.ParameterInstance;

public class ParameterInstanceFactory<
    TInstance,
    TParameter> : IDisposable,
    IAsyncDisposable,
    IParameterInstanceFactory<TInstance, TParameter>
    where TInstance : notnull
    where TParameter : notnull
{
    private bool _disposed;
    private readonly ConcurrentDictionary<TParameter, Lazy<TInstance>> _instances;
    private readonly IServiceProvider _provider;

    public ParameterInstanceFactoryOptions Options { get; }

    public ParameterInstanceFactory(IServiceProvider provider)
    {
        _provider = provider;
        _instances = new();
        Options = provider.GetRequiredService<IOptionsMonitor<ParameterInstanceFactoryOptions>>().CurrentValue;
        Options.Add(this);
    }

    public virtual TInstance GetOrCreate(TParameter parameter)
    {
        return _instances.GetOrAdd(parameter,
                static (parameter, factory) =>
                    new(() => factory.InstanceFactory(factory._provider, parameter),
                        LazyThreadSafetyMode.ExecutionAndPublication),
                this)
            .Value;
    }

    protected virtual TInstance InstanceFactory(IServiceProvider provider, TParameter parameter)
    {
        return ActivatorUtilities.CreateInstance<TInstance>(provider, parameter);
    }

    public virtual bool TryRemove(TParameter parameter)
    {
        if (!_instances.TryRemove(parameter, out var instance))
        {
            return false;
        }

        if (instance is { IsValueCreated: true, Value: IDisposable disposable })
        {
            disposable.Dispose();
        }

        return true;
    }

    public IEnumerator<TInstance> GetEnumerator()
    {
        return _instances.Values.Select(x => x.Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Clear()
    {
        DisposeCore();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        DisposeCore();
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    protected void DisposeCore()
    {
        if (typeof(TInstance).IsAssignableTo(typeof(IDisposable)))
        {
            foreach (var keyValuePair in _instances.Values
                         .Where(x => x.IsValueCreated)
                         .Select(x => x.Value as IDisposable))
            {
                keyValuePair?.Dispose();
            }
        }

        _instances.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    public async ValueTask DisposeAsyncCore()
    {
        if (typeof(TInstance).IsAssignableTo(typeof(IAsyncDisposable)))
        {
            foreach (var keyValuePair in _instances.Values
                         .Where(x => x.IsValueCreated)
                         .Select(x => x.Value as IAsyncDisposable))
            {
                if (keyValuePair != null)
                {
                    await keyValuePair.DisposeAsync();
                }
            }
        }

        _instances.Clear();
    }
}