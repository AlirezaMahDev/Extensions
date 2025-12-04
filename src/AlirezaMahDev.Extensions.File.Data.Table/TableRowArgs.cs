using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Table;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly record struct TableRowArgs(TableRows Rows, int Index);