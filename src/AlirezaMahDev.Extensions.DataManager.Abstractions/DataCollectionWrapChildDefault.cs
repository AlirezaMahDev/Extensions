namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataCollectionWrapChildDefault<TValue>
    where TValue : unmanaged, IDataCollection<TValue>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ref DataOffset RefChild(ref TValue x)
    {
        return ref x.Child;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ref readonly DataOffset RefReadOnlyChild(ref readonly TValue x)
    {
        return ref x.Child;
    }
}