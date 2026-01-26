using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data;

[StructLayout(LayoutKind.Sequential)]
internal record struct DataLocationArgs(DataAccess DataAccess, long Id);