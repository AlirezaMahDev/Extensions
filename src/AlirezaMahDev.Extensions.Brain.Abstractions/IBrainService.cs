namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface IBrainService
{
    INerve<TData, TLink> GetOrAdd<TData, TLink>(string name)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>;

    INerve<TData, TLink> GetOrAddTemp<TData, TLink>(string? name = null)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>;
}