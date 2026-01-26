using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public record struct CollectionObjectValue
{
    public DateTimeOffset DeleteAt;
}