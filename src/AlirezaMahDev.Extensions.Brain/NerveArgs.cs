using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
readonly record struct NerveArgs<TData>(Nerve<TData> Nerve, long Offset)
    where TData : unmanaged
{
    public Nerve<TData> Nerve { get; } = Nerve;
}