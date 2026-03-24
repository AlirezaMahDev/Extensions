using System.Collections.Concurrent;

namespace AlirezaMahDev.Extensions.ParameterInstance.Abstractions;

public class ParameterInstanceFactoryOptions
{
    public ConcurrentDictionary<Type, ConcurrentBag<object>> Factories { get; } = [];

    public void Add<TParameter>(IParameterInstanceFactory<TParameter> factory)
        where TParameter : notnull

    {
        Factories.GetOrAdd(typeof(TParameter), _ => []).Add(factory);
    }

    public IEnumerable<IParameterInstanceFactory<TParameter>> GetFactories<TParameter>()
        where TParameter : notnull
    {
        return Factories.TryGetValue(typeof(TParameter), out var bag)
            ? bag.Cast<IParameterInstanceFactory<TParameter>>()
            : [];
    }
}