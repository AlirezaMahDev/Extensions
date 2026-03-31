namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public struct NerveCounter : IDataValue<NerveCounter>, IDataValueDefault<NerveCounter>
{
    public int NeuronCount;
    public int ConnectionCount;

    private DataLock _lock;

    public readonly ref DataLock Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._lock;
    }

    private static readonly NerveCounter DefaultField = new()
    {
        NeuronCount = 0,
        ConnectionCount = 0
    };

    public static ref readonly NerveCounter Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref DefaultField;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool Equals(scoped ref readonly NerveCounter other)
    {
        return Vector64.LoadUnsafe(ref Unsafe.As<NerveCounter, byte>(ref Unsafe.AsRef(in this))) ==
               Vector64.LoadUnsafe(ref Unsafe.As<NerveCounter, byte>(ref Unsafe.AsRef(in other)));
    }
}