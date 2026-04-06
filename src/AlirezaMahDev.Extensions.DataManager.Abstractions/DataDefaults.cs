namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataDefaults
{
    public const string FileFormat = "{0}.db";
    public const int FileCount = 512;
    public const int FileSize = 1 << 30;
    public const int PartSize = 1 << 16;
    public const int PartCount = FileSize / PartSize;
    public const long AllocMax = 1L << 30;
    public const long AllocHigh = AllocMax - (AllocMax / 8);
    public const long AllocNormal = AllocMax - (AllocMax / 4);
}