using AlirezaMahDev.Extensions.Brain.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.Brain;

internal class BrainService(IServiceProvider provider) : IBrainService
{
    public INerve<TData, TLink> GetOrAdd<TData, TLink>(string name)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        return provider.GetRequiredService<NerveFactory<TData, TLink>>().GetOrCreate(name);
    }

    public INerve<TData, TLink> GetOrAddTemp<TData, TLink>(string? name = null)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        return provider.GetRequiredService<NerveFactory<TData, TLink>>()
            .GetOrCreate($"temp:{name ?? Guid.CreateVersion7().ToString()}");
    }
}