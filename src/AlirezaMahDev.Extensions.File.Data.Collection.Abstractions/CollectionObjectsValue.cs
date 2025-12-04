using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct CollectionObjectsValue
{
    public int Count;
}