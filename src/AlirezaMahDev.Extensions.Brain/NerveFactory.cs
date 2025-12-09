using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

class NerveFactory<TData,TLink>(
    IServiceProvider provider)
    : ParameterInstanceFactory<Nerve<TData,TLink>, string>(provider)
    where TData : unmanaged
    where TLink : unmanaged;