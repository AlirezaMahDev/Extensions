using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public record struct NerveCounter : IDataValue<NerveCounter>, IDataValueDefault<NerveCounter>
{
    public int NeuronCount;
    public int ConnectionCount;
    public static NerveCounter Default => new()
    {
        NeuronCount = 0,
        ConnectionCount = 0
    };
}