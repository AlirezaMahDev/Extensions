using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct DataBlockMemoryObject
{
    public DataBlockMemory Memory { get; }

    public DataBlockMemoryObject(DataBlockMemory memory,[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]  Type type)
    {
        Memory = memory;
        var blockValueType = typeof(DataBlockMemoryValue<>).MakeGenericType(type);
        var blockValueTypeConstructor = blockValueType.GetConstructor([typeof(DataBlockMemory)])!;
        BlockValue = blockValueTypeConstructor.Invoke([memory]);
        var blockValueProperty = blockValueType.GetProperty(nameof(DataBlockMemoryValue<>.Value))!;
        GetMethodInfo = blockValueProperty.GetMethod!;
        SetMethodInfo = blockValueProperty.SetMethod!;
    }

    private object BlockValue { get; }
    private MethodInfo GetMethodInfo { get; }
    private MethodInfo SetMethodInfo { get; }

    public object? Value
    {
        get => GetMethodInfo.Invoke(BlockValue, []);
        set => SetMethodInfo.Invoke(BlockValue, [value]);
    }
}