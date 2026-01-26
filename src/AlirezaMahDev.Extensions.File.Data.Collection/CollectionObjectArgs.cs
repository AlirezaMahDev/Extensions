using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct CollectionObjectArgs(CollectionObjects Objects, int Index);