using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public record struct TableColumnValue
{
    public DateTimeOffset DeleteAt;
}