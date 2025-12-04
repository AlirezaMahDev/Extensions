using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly record struct CollectionObjectArgs(CollectionObjects Objects, int Index);