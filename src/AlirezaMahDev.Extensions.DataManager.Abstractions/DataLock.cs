namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Size = 8, Pack = 1)]
public struct DataLock
{
    public int Thread;
    public ushort Session;
    public short State;
}