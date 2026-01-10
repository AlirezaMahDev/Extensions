using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
record struct DataAccessValue : IDataValue<DataAccessValue>
{
    public long LastOffset;
}