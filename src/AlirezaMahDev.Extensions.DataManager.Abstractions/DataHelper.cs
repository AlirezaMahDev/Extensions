namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int FileId(long offset)
    {
        return (int)(offset / DataDefaults.FileSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int FileOffset(long offset)
    {
        return (int)(offset % DataDefaults.FileSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int PartIndex(long offset)
    {
        return FileOffset(offset) / DataDefaults.PartSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int PartOffset(long offset)
    {
        return (int)(offset % DataDefaults.PartSize);
    }
}