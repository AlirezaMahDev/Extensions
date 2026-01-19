using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

class NerveFactory<TData, TLink>(
    IServiceProvider provider)
    : ParameterInstanceFactory<Nerve<TData, TLink>, string>(provider)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>;