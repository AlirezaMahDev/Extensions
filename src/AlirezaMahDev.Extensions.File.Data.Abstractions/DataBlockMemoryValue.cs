using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct DataBlockMemoryValue<TValue>(DataBlockMemory memory)
    where TValue : unmanaged
{
    public DataBlockMemory Memory { get; } = memory;

    public TValue Value
    {
        get => MemoryMarshal.Read<TValue>(Memory.Memory.Span);
        set => MemoryMarshal.Write(Memory.Memory.Span, value);
    }
}