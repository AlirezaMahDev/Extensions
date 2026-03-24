namespace AlirezaMahDev.Extensions.Brain;

internal class NerveFactory<TData, TLink>(
    IServiceProvider provider)
    : ParameterInstanceFactory<Nerve<TData, TLink>, string>(provider)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>;