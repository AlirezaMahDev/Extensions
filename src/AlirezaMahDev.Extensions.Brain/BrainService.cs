using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.Brain;

internal class BrainService(IServiceProvider provider) : IBrainService
{
    public Nerve<TData> GetOrAdd<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TData>(string name)
        where TData : unmanaged, IEquatable<TData>
    {
        return provider.GetRequiredService<NerveFactory<TData>>().GetOrCreate(name);
    }
}