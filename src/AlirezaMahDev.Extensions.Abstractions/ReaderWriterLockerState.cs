namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential, Size = 8, Pack = 1)]
public struct ReaderWriterLockerState
{
    public int ThreadId;
    public short WriterCount;
    public ushort ReaderCount;
}