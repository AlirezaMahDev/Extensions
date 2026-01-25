namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataDefaults
{
    public const string FileFormat = "{0}.db";
    public const int PartSize = 1 << 28;
    public const int PartCount = 4;
    public const int FileSize = 1 << 30;
    public const int FileCount = 64;
}