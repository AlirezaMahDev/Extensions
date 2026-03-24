namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Connection(in DataOffset offset) : ICell, IInEquatable<Connection>
{
    private readonly DataOffset _offset = offset;

    public ref readonly DataOffset Offset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this)._offset;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(in Connection other)
    {
        return _offset == other._offset;
    }
}