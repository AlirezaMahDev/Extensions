namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly record struct DataOffset(int FileId, int PartIndex, int Offset, int Length)
{
    public bool IsNull => this == Null;
    public bool IsDefault => this == Default;

    public static DataOffset Null { get; } = new(-1, -1, -1, -1);
    public static DataOffset Default { get; } = new();

    public static DataOffset Create(long offset, int length) =>
        new(DataHelper.FileId(offset), DataHelper.PartIndex(offset), DataHelper.PartOffset(offset), length);
}