using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
readonly record struct NerveArgs<TData,TLink>(Nerve<TData,TLink> Nerve, long Offset)
    where TData : unmanaged
    where TLink : unmanaged
{
    public Nerve<TData,TLink> Nerve { get; } = Nerve;
}