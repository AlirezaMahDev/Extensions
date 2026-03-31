namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataCollectionWrapChildDefault<TValue>
    where TValue : unmanaged, IDataCollection<TValue>
{
    public static readonly ScopedRefValueFunc<TValue, DataOffset> RefChild =
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        (scoped ref x) => ref x.Child;
    public static readonly ScopedRefReadOnlyValueFunc<TValue, DataOffset> RefReadOnlyChild =
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        (scoped ref readonly x) => ref x.Child;
}