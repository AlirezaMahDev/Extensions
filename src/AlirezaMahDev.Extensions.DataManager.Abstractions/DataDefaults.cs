namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataDefaults
{
    public const string FileFormat = "{0}.db";
    public const int FileCount = 512;
    public const int FileSize = 1 << 26;
    public const int PartSize = 1 << 14;
    public const int PartCount = FileSize / PartSize;
    public const long AllocMax = 1L << 31;
    public const long AllocPressure = AllocMax / 8;
    public const long AllocSafe = AllocMax - AllocPressure;
    public const float AllocDeadTicksFactor = 0.55f;
}
