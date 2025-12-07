namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface IBrainService
{
    INerve<TData> GetOrAdd<TData>(string name)
        where TData : unmanaged, IEquatable<TData>;
}