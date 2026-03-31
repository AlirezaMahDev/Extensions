namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataCollectionWrapNextDefault<TValue>
    where TValue : unmanaged, IDataCollectionItem<TValue>
{
    public static readonly ScopedRefValueFunc<TValue, DataOffset> RefNext =
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        (scoped ref x) => ref x.Next;

    public static readonly ScopedRefReadOnlyValueFunc<TValue, DataOffset> RefReadOnlyNext =
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        (scoped ref readonly x) => ref x.Next;
}