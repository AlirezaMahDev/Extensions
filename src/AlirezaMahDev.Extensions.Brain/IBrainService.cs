namespace AlirezaMahDev.Extensions.Brain;

public interface IBrainService
{
    Nerve<TData> GetOrAdd<TData>(string name)
        where TData : unmanaged, IEquatable<TData>;
}