namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface IBrainService
{
    INerve<TData,TLink> GetOrAdd<TData,TLink>(string name)
        where TData : unmanaged, IEquatable<TData>
        where TLink : unmanaged, IEquatable<TLink>;
}