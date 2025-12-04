using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

public class ConnectionFactory<TData>(
    IServiceProvider provider,
    Nerve<TData> nerve)
    : ParameterInstanceFactory<Connection<TData>, NerveArgs<TData>>(provider), IDataBlockAccessorSave
    where TData : unmanaged
{
    public StackAccess<ConnectionValue> ConnectionStack { get; } =
        nerve.Location.GetOrAdd(".connections").AsStack().As<ConnectionValue>();

    public Connection<TData> GetOrCreate(int id)
    {
        return GetOrCreate(new NerveArgs<TData>(nerve, id));
    }

    public void Save()
    {
        ConnectionStack.Stack.Save();
    }
}