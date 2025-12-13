using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.Brain;

internal class BrainService(IServiceProvider provider) : IBrainService
{
    public INerve<TData, TLink> GetOrAdd<TData, TLink>(string name)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        return provider.GetRequiredService<NerveFactory<TData, TLink>>().GetOrCreate(name);
    }

    public INerve<TData, TLink> GetOrAddTemp<TData, TLink>(string? name = null)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        return provider.GetRequiredService<NerveFactory<TData, TLink>>()
            .GetOrCreate($"temp:{name ?? Guid.CreateVersion7().ToString()}");
    }
}