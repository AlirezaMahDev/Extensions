namespace AlirezaMahDev.Extensions.Brain;

public class RootConnection<TData>(Nerve<TData> nerve, Neuron<TData> neuron)
    : Connection<TData>(new(nerve, 0), neuron, default)
    where TData : unmanaged;