namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataHelper
{
    public static int FileId(long offset)
    {
        return (int)(offset / DataDefaults.FileSize);
    }

    public static int FileOffset(long offset)
    {
        return (int)(offset % DataDefaults.FileSize);
    }

    public static int PartIndex(long offset)
    {
        return FileOffset(offset) / DataDefaults.PartSize;
    }

    public static int PartOffset(long offset)
    {
        return (int)(offset % DataDefaults.PartSize);
    }
}