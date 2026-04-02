namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataCollectionWrapNextDefault<TValue>
    where TValue : unmanaged, IDataCollectionItem<TValue>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ref DataOffset RefNext(ref TValue x) => ref x.Next;
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ref readonly DataOffset RefReadOnlyNext(ref readonly TValue x) => ref x.Next;
}