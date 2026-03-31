namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public class DataDictionaryWrap<TValue, TItem, TKey, TItemWrap>(
    ScopedRefValueFunc<TValue, DataOffset> refChild,
    ScopedRefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyChild,
    TItemWrap itemWrap)
    : DataCollectionWrap<TValue, TItem, DataDictionaryItemWrap<TItem, TKey>>(refChild, refReadOnlyChild, itemWrap)
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
    where TItemWrap : DataDictionaryItemWrap<TItem, TKey>
{
}