using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

class NeuronFactory<TData, TLink>(IServiceProvider provider, Nerve<TData, TLink> nerve)
    : ParameterInstanceFactory<Neuron<TData, TLink>, NeuronArgs<TData, TLink>>(provider)
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public DataLocation<DataPath> Location { get; } = nerve.Location.Wrap(x => x.Dictionary()).GetOrAdd(".neuron");

    public Neuron<TData, TLink> GetOrCreate(long offset) =>
        GetOrCreate(new NeuronArgs<TData, TLink>(nerve, Location.Access.Read<NeuronValue<TData>>(offset)));

    public async ValueTask<Neuron<TData, TLink>> GetOrCreateAsync(long offset, CancellationToken cancellationToken = default) =>
        GetOrCreate(new NeuronArgs<TData, TLink>(nerve, await Location.Access.ReadAsync<NeuronValue<TData>>(offset, cancellationToken)));
}