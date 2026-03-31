using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

internal sealed class DataMapFilePartOwner : IDataMapFilePartOwner
{
    private readonly DataMapFilePart _dataMapFilePart;

    internal DataMapFilePartOwner(DataMapFilePart dataMapFilePart)
    {
        _dataMapFilePart = dataMapFilePart;
        Interlocked.Increment(ref _dataMapFilePart.AccessCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ref byte GetRef(int offset)
    {
        return ref _dataMapFilePart.GetRef(offset);
    }

    ~DataMapFilePartOwner()
    {
        Interlocked.Decrement(ref _dataMapFilePart.AccessCount);
    }
}
