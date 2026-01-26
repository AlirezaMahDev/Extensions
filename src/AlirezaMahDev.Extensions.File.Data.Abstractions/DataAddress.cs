using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
[StructLayout(LayoutKind.Sequential)]
public readonly record struct DataAddress(long Id, int Length)
{
    public readonly long Id = Id;
    public readonly int Length = Length;

    public long NextId => Id + Length;

    private string GetDebuggerDisplay()
    {
        return Id.ToString();
    }
}