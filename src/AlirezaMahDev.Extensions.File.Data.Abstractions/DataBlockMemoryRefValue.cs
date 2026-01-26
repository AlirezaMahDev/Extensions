using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct DataBlockMemoryRefValue<TValue>(DataBlockMemory blockMemory)
    where TValue : unmanaged
{
    public DataBlockMemory BlockMemory { get; } = blockMemory;

    public ref TValue RefValue => ref MemoryMarshal.AsRef<TValue>(BlockMemory.Memory.Span);
}