using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct DataBlockMemory(IDataBlockAccessor accessor, Memory<byte> memory)
{
    public DataBlockMemory(IDataBlockAccessor accessor) : this(accessor, accessor.Memory)
    {
    }

    public IDataBlockAccessor Accessor { get; } = accessor;
    public Memory<byte> Memory { get; } = memory;

    public DataBlockMemory this[Range range] => new(Accessor, Memory[range]);

    public DataBlockMemory Slice(int start, int length) => new(Accessor, Memory.Slice(start, length));

    public DataBlockMemory Reset()
    {
        return new(Accessor, Accessor.Memory);
    }
}