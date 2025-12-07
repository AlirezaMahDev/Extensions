using AlirezaMahDev.Extensions.DataManager.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

class ConnectionFactory<TData>(
    IServiceProvider provider,
    Nerve<TData> nerve)
    : ParameterInstanceFactory<Connection<TData>, NerveArgs<TData>>(provider)
    where TData : unmanaged
{
    public DataLocation<DataPath> Location { get; } = nerve.Location.Wrap(x => x.Dictionary()).GetOrAdd(".connection");

    public Connection<TData> GetOrCreate(long offset)
    {
        return GetOrCreate(new NerveArgs<TData>(nerve, offset));
    }
}