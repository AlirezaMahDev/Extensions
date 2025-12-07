using AlirezaMahDev.Extensions.Brain.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.Brain;

internal class BrainService(IServiceProvider provider) : IBrainService
{
    public INerve<TData> GetOrAdd<TData>(string name)
        where TData : unmanaged, IEquatable<TData>
    {
        return provider.GetRequiredService<NerveFactory<TData>>().GetOrCreate(name);
    }
}