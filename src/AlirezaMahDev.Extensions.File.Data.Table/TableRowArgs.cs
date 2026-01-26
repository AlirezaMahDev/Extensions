using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Table;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct TableRowArgs(TableRows Rows, int Index);