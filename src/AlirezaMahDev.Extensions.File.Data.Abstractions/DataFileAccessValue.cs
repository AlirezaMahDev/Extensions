using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public record struct DataFileAccessValue
{
    public long Last;
}