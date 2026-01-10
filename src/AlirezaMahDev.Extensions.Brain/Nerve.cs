using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class Nerve<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TData,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TLink> : INerve<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public IDataAccess Access { get; }
    public INerveCache<TData, TLink> Cache { get; }

    public string Name { get; }

    public DataLocation<DataPath> Location { get; }
    public DataLocation<DataPath> ConnectionLocation { get; }
    public DataLocation<DataPath> NeuronLocation { get; }

    public Neuron<TData, TLink> Neuron { get; }
    public NeuronWrap<TData, TLink> NeuronWrap { get; }

    public Connection<TData, TLink> Connection { get; }
    public ConnectionWrap<TData, TLink> ConnectionWrap { get; }

    public Nerve(IDataManager dataManager, string name)
    {
        Cache = new NerveCache<TData, TLink>();

        Name = name;
        Access = Name.StartsWith("temp:") ? dataManager.OpenTemp() : dataManager.Open(name);
        Location = Access.Root.Wrap(Access, x => x.Dictionary()).GetOrAdd(".nerve");
        ConnectionLocation = Location.Wrap(Access, x => x.Dictionary()).GetOrAdd(".connection");
        NeuronLocation = Location.Wrap(Access, x => x.Dictionary()).GetOrAdd(".neuron");

        var neuron = NeuronLocation
            .Wrap(Access, x => x.Storage())
            .GetOrCreateData(NeuronValue<TData>.Default);
        var connection = ConnectionLocation
            .Wrap(Access, x => x.Storage())
            .GetOrCreateData(ConnectionValue<TLink>.Default with { Neuron = neuron.Offset });

        Neuron = new(neuron.Offset);
        NeuronWrap = Neuron.Wrap(this);
        Connection = new(connection.Offset);
        ConnectionWrap = Connection.Wrap(this);
    }

    public void Flush()
    {
        Access.Flush();
    }
}