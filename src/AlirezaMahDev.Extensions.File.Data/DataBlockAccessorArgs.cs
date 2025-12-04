using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal record struct DataBlockAccessorArgs(DataBlock DataBlock, DataAddress Address);