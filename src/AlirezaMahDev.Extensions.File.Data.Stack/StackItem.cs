using System.Numerics;
using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Stack;

internal class StackItem : IStackItem
{
    private readonly DataBlockMemory _entityBlockMemory;
    private readonly DataBlockMemoryRefValue<StackItemValue> _databaseBlockMemoryRefValue;

    public StackItem(StackItemArgs args)
    {
        Index = args.Index;

        Capacity = (int)BitOperations.RoundUpToPowerOf2((uint)args.Index + 1) >> 1;
        SubIndex = args.Index - Capacity;

        var size = Unsafe.SizeOf<StackItemValue>() + args.Items.StackAccess.Size;
        var location = args.Items.LocationValue.Location.GetOrAdd(Capacity.ToString(), Capacity * size);

        location.Data.AsMemory()
            .Slice(SubIndex * size, size)
            .AsReader()
            .ReadRefValue(out _databaseBlockMemoryRefValue)
            .ReadBlock(out _entityBlockMemory);
    }


    public int Index { get; }
    public int Capacity { get; }
    public int SubIndex { get; }

    public void Remove()
    {
        _databaseBlockMemoryRefValue.RefValue.DeleteAt = DateTimeOffset.UtcNow;
    }

    public DataBlockMemory Data => _entityBlockMemory;
    public StackItemValue Value => _databaseBlockMemoryRefValue.RefValue;
}