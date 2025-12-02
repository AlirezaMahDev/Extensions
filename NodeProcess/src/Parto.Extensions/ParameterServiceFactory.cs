using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Parto.Extensions.Abstractions;

namespace Parto.Extensions;

public class ParameterServiceFactory<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TInstance,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TParameter> : IDisposable,
    IAsyncDisposable,
    IParameterServiceFactory<TInstance, TParameter>
    where TInstance : notnull
    where TParameter : notnull
{
    private bool _disposed;
    private readonly ConcurrentDictionary<TParameter, Lazy<TInstance>> _instances;
    private readonly IServiceProvider _provider;

    public ParameterServiceFactoryOptions Options { get; }

    public ParameterServiceFactory(IServiceProvider provider)
    {
        _provider = provider;
        _instances = new();
        Options = provider.GetRequiredService<IOptionsMonitor<ParameterServiceFactoryOptions>>().CurrentValue;
        Options.Add(this);
    }

    public virtual TInstance GetOrCreate(TParameter parameter)
    {
        return _instances.GetOrAdd(parameter,
                static (parameter, provider) =>
                    new(() => ActivatorUtilities.CreateInstance<TInstance>(provider, parameter),
                        LazyThreadSafetyMode.ExecutionAndPublication),
                _provider)
            .Value;
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
            foreach (var keyValuePair in _instances.Values.Select(x => x.Value).Cast<IDisposable>())
            {
                keyValuePair.Dispose();
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
            foreach (var keyValuePair in _instances.Values.Select(x => x.Value).Cast<IAsyncDisposable>())
            {
                await keyValuePair.DisposeAsync();
            }
        }

        _instances.Clear();
    }
}