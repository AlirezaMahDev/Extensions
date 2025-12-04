using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

internal class NerveFactory<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TData>(
    IServiceProvider provider)
    : ParameterInstanceFactory<Nerve<TData>, string>(provider)
    where TData : unmanaged;