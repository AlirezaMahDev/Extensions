using System.Collections.Concurrent;

namespace Parto.Extensions.Abstractions;

public class ParameterServiceFactoryOptions
{
    public ConcurrentDictionary<Type, ConcurrentBag<object>> Factories { get; } = [];

    public void Add<TParameter>(IParameterServiceFactory<TParameter> factory)
        where TParameter : notnull

    {
        Factories.GetOrAdd(typeof(TParameter), _ => []).Add(factory);
    }

    public IEnumerable<IParameterServiceFactory<TParameter>> GetFactories<TParameter>()
        where TParameter : notnull
    {
        return Factories.TryGetValue(typeof(TParameter), out var bag)
            ? bag.Cast<IParameterServiceFactory<TParameter>>()
            : [];
    }
}