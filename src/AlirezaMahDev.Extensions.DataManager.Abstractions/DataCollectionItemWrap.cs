namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct DataCollectionItemWrap<TValue>(GetRefValueFunc<TValue, DataOffset> getRefNext)
    where TValue : unmanaged, IDataValue<TValue>
{
    public GetRefValueFunc<TValue, DataOffset> GetRefNext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = getRefNext;


    public GetValueFunc<TValue, DataOffset> GetValueNext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = (ref value) => getRefNext(ref value);
}