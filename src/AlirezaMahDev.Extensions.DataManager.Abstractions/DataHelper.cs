namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataHelper
{
    public static int FileId(long offset) => (int)(offset / DataDefaults.FileSize);
    public static int FileOffset(long offset) => (int)(offset % DataDefaults.FileSize);

    public static int PartIndex(long offset) => FileOffset(offset) / DataDefaults.PartSize;
    public static int PartOffset(long offset) => (int)(offset % DataDefaults.PartSize);
}
