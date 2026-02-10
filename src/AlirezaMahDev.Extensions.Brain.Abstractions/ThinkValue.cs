using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly record struct ThinkValue<TData, TLink>(
    ReadOnlyMemoryValue<TData> Data,
    ReadOnlyMemoryValue<TLink> Link,
    float Score = float.MaxValue,
    uint Weight = uint.MaxValue)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>;