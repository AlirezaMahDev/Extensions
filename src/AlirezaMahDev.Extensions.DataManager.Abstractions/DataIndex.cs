using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public record struct DataIndex(String64 Key);