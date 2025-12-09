using AlirezaMahDev.Extensions.Brain.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.Brain;

internal class BrainService(IServiceProvider provider) : IBrainService
{
    public INerve<TData, TLink> GetOrAdd<TData, TLink>(string name)
        where TData : unmanaged, IEquatable<TData>
        where TLink : unmanaged, IEquatable<TLink>
    {
        return provider.GetRequiredService<NerveFactory<TData, TLink>>().GetOrCreate(name);
    }
}