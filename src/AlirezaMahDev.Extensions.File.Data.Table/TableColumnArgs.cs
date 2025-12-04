using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly record struct TableColumnArgs(TableColumns Columns, String64 Key);