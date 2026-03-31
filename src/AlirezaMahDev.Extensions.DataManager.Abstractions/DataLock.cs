namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public struct DataLock
{
    public uint Session;
    public int State;
}