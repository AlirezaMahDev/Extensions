using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

[StructLayout(LayoutKind.Sequential)]
record struct DataAccessValue : IDataValue<DataAccessValue>
{
    public long LastOffset;
}