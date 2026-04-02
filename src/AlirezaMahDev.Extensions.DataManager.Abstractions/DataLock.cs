namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public struct DataLock
{
    public int Thread;
    public ushort Session;
    public short State;
}