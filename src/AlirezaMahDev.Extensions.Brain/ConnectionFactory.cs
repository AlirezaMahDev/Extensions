using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

class ConnectionFactory<TData, TLink>(
    IServiceProvider provider,
    Nerve<TData, TLink> nerve)
    : ParameterInstanceFactory<Connection<TData, TLink>, ConnectionArgs<TData, TLink>>(provider)
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public DataLocation<DataPath> Location { get; } =
        nerve.Location.Wrap(x => x.Dictionary()).GetOrAdd(".connection");

    public Connection<TData, TLink> GetOrCreate(long offset) =>
        GetOrCreate(new ConnectionArgs<TData, TLink>(nerve, Location.Access.Read<ConnectionValue<TLink>>(offset)));
        
    public async ValueTask<Connection<TData, TLink>> GetOrCreateAsync(long offset, CancellationToken cancellationToken = default) =>
        GetOrCreate(new ConnectionArgs<TData, TLink>(nerve, await Location.Access.ReadAsync<ConnectionValue<TLink>>(offset, cancellationToken)));
}