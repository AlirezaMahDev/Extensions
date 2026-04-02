namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataCollectionItemWrap<TValue>(
    RefValueFunc<TValue, DataOffset> getRefNext,
    RefReadOnlyValueFunc<TValue, DataOffset> getRefReadOnlyNext)
    where TValue : unmanaged, IDataValue<TValue>
{

    public RefReadOnlyValueFunc<TValue, DataOffset> RefReadOnlyNext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = getRefReadOnlyNext;

    public RefValueFunc<TValue, DataOffset> RefNext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = getRefNext;
}