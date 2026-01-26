using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public record struct AllocateMemory<T>(DataOffset Offset, Memory<T> Memory);