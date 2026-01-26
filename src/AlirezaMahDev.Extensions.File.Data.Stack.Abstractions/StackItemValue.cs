using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public record struct StackItemValue
{
    public DateTimeOffset DeleteAt;
}