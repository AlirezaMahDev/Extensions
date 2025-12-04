using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct TableColumnValue
{
    public DateTimeOffset DeleteAt;
}