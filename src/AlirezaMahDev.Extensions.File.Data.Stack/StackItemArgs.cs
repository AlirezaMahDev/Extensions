using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Stack;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct StackItemArgs(StackItems Items, int Index)
{
    public StackItems Items { get; } = Items;
    public int Index { get; } = Index;
}