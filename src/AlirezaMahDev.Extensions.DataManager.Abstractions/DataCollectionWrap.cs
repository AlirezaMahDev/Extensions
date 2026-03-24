namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct DataCollectionWrap<TValue, TItem>(
    GetRefValueFunc<TValue, DataOffset> getRefChild,
    GetRefValueFunc<TItem, DataOffset> getRefNext)
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
{
    public GetRefValueFunc<TValue, DataOffset> GetRefChild
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = getRefChild;


    public GetValueFunc<TValue, DataOffset> GetChild
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = (ref value) => getRefChild(ref value);

    public DataCollectionItemWrap<TItem> ItemWrap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = new(getRefNext);
}