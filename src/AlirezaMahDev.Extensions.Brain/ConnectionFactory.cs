using AlirezaMahDev.Extensions.DataManager.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

class ConnectionFactory<TData,TLink>(
    IServiceProvider provider,
    Nerve<TData,TLink> nerve)
    : ParameterInstanceFactory<Connection<TData,TLink>, NerveArgs<TData,TLink>>(provider)
    where TData : unmanaged
    where TLink : unmanaged
{
    public DataLocation<DataPath> Location { get; } = nerve.Location.Wrap(x => x.Dictionary()).GetOrAdd(".connection");

    public Connection<TData,TLink> GetOrCreate(long offset)
    {
        return GetOrCreate(new NerveArgs<TData,TLink>(nerve, offset));
    }
}