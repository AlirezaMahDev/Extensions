using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public record struct DataIndexStorage(String64 Key);