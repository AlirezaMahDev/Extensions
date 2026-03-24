namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public struct NerveCounter : IDataValue<NerveCounter>, IDataValueDefault<NerveCounter>
{
    public int NeuronCount;
    public int ConnectionCount;

    private static readonly NerveCounter DefaultField = new()
    {
        NeuronCount = 0,
        ConnectionCount = 0
    };

    public static ref readonly NerveCounter Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref DefaultField;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(in NerveCounter other) =>
        Vector64.LoadUnsafe(ref Unsafe.As<NerveCounter, byte>(ref Unsafe.AsRef(in this))) ==
        Vector64.LoadUnsafe(ref Unsafe.As<NerveCounter, byte>(ref Unsafe.AsRef(in other)));
}