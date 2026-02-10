using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly record struct PredictValue<TLink>(
    ReadOnlyMemoryValue<TLink> Link,
    float Score = float.MaxValue,
    uint Weight = uint.MaxValue)
    where TLink : unmanaged, ICellLink<TLink>;