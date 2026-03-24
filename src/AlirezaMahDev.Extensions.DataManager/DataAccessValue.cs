using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

[StructLayout(LayoutKind.Sequential)]
internal struct DataAccessValue : IDataValue<DataAccessValue>
{
    public long LastOffset;

    public bool Equals(in DataAccessValue other)
    {
        return LastOffset == other.LastOffset;
    }
}