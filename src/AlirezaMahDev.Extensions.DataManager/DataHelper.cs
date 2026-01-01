namespace AlirezaMahDev.Extensions.DataManager;

class DataHelper
{
    public static long FileId(long offset) => offset / DataDefaults.FileSize;
    public static int FileOffset(long offset) => (int)(offset % DataDefaults.FileSize);

    public static int PartIndex(long offset) => FileOffset(offset) / DataDefaults.PartSize * DataDefaults.PartSize;
    public static int PartOffset(long offset) => (int)(offset % DataDefaults.PartSize);
}
