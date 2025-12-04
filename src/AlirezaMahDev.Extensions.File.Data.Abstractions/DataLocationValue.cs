using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct DataLocationValue
{
    public String64 Key;
    public int Length;
    public long Child;
    public long Next;
}