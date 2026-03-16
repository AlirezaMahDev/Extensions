using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct DataBlockMemoryObject
{
    public DataBlockMemory Memory { get; }

    public DataBlockMemoryObject(DataBlockMemory memory,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type type)
    {
        Memory = memory;
        Type blockValueType = typeof(DataBlockMemoryValue<>).MakeGenericType(type);
        ConstructorInfo blockValueTypeConstructor = blockValueType.GetConstructor([typeof(DataBlockMemory)])!;
        BlockValue = blockValueTypeConstructor.Invoke([memory]);
        PropertyInfo blockValueProperty = blockValueType.GetProperty(nameof(DataBlockMemoryValue<>.Value))!;
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