namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataCollectionItemWrap<TValue>(
    ScopedRefValueFunc<TValue, DataOffset> getRefNext,
    ScopedRefReadOnlyValueFunc<TValue, DataOffset> getRefReadOnlyNext)
    where TValue : unmanaged, IDataValue<TValue>
{

    public ScopedRefReadOnlyValueFunc<TValue, DataOffset> RefReadOnlyNext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = getRefReadOnlyNext;

    public ScopedRefValueFunc<TValue, DataOffset> RefNext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = getRefNext;
}