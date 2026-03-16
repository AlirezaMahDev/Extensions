using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

[StructLayout(LayoutKind.Sequential)]
internal record struct DataAccessValue : IDataValue<DataAccessValue>
{
    public long LastOffset;
}