namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataDictionaryWrapKeyDefault<TValue, TKey>
    where TValue : unmanaged, IDataDictionaryItem<TValue, TKey>
    where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ref TKey RefKey(ref TValue value) => ref value.Key;
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ref readonly TKey RefReadOnlyKey(ref readonly TValue value) => ref value.Key;
}