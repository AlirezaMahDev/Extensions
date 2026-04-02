using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    public ref byte GetRef(scoped in DataOffset offset)
    {
        return ref _dataMapFilePart.GetRef(offset.Offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Span<byte> GetSpan(scoped in DataOffset offset)
    {
        return MemoryMarshal.CreateSpan(ref GetRef(offset), offset.Length);
    }

    ~DataMapFilePartOwner()
    {
        Interlocked.Decrement(ref _dataMapFilePart.AccessCount);
    }
}