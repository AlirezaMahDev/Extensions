using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct StackAccessValue
{
    public int Size;
}