using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct CollectionPropertyArgs(CollectionProperties Properties, String64 Key);