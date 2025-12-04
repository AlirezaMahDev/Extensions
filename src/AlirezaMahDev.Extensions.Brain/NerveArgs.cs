using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct NerveArgs<TData>(Nerve<TData> Nerve, int Id)
    where TData : unmanaged
{
    public Nerve<TData> Nerve { get; } = Nerve;
}