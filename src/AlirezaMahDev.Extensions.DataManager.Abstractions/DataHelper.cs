namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int FileId(in long offset)
    {
        return (int)(offset / DataDefaults.FileSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int FileOffset(in long offset)
    {
        return (int)(offset % DataDefaults.FileSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int PartIndex(in long offset)
    {
        return FileOffset(offset) / DataDefaults.PartSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int PartOffset(in long offset)
    {
        return (int)(offset % DataDefaults.PartSize);
    }
}