using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public readonly record struct DataLocationObjectProperty
{
    public PropertyInfo PropertyInfo { get; }

    private readonly Action<object?> _setValue;
    private readonly Func<object?> _getValue;

    public DataLocationObjectProperty(DataBlockMemory dataBlockMemory, PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo;
        if (!propertyInfo.PropertyType.IsUnmanaged)
        {
            throw new InvalidCastException($"property {propertyInfo.Name} is not unmanaged");
        }

        Func<DataBlockMemory, object> createMemoryValue = CreateMemoryValue(propertyInfo,
            out Func<object, object> getValue,
            out Action<object, object> valueSetter);

        _setValue = value =>
        {
            if (value is null)
            {
                throw new NotSupportedException("Cannot set null value");
            }

            object memoryValue = createMemoryValue(dataBlockMemory);
            valueSetter(memoryValue, value);
        };

        _getValue = () =>
        {
            object memoryValue = createMemoryValue(dataBlockMemory);
            return getValue(memoryValue);
        };
    }

    public DataLocationObjectProperty(IDataLocation location, PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo;
        if (!propertyInfo.PropertyType.IsUnmanaged)
        {
            throw new InvalidCastException($"property {propertyInfo.Name} is not unmanaged");
        }

        Func<DataBlockMemory, object> createMemoryValue = CreateMemoryValue(propertyInfo,
            out Func<object, object> getValue,
            out Action<object, object> valueSetter);

        _setValue = value =>
        {
            if (value is null)
            {
                throw new NotSupportedException("Cannot set null value");
            }

            object memoryValue = createMemoryValue(location.NewData(Marshal.SizeOf(value)));
            valueSetter(memoryValue, value);
        };

        _getValue = () =>
        {
            DataBlockMemory? dataBlockMemory = location.LastData();
            if (!dataBlockMemory.HasValue || dataBlockMemory.Value.Memory.IsEmpty)
            {
                return Activator.CreateInstance(propertyInfo.PropertyType);
            }

            object memoryValue = createMemoryValue(dataBlockMemory.Value);
            return getValue(memoryValue);
        };
    }

    private static Func<DataBlockMemory, object> CreateMemoryValue(PropertyInfo propertyInfo,
        out Func<object, object> getValue,
        out Action<object, object> setValue)
    {
        Type memoryValueType = typeof(DataBlockMemoryValue<>).MakeGenericType(propertyInfo.PropertyType);
        PropertyInfo memoryValueProperty = memoryValueType.GetProperty(nameof(DataBlockMemoryValue<>.Value))!;

        ParameterExpression parameter = Expression.Parameter(typeof(DataBlockMemory));
        Func<DataBlockMemory, object> createMemoryValue = Expression.Lambda<Func<DataBlockMemory, object>>(
                Expression.Convert(
                    Expression.New(
                        memoryValueType.GetConstructor([typeof(DataBlockMemory)])!,
                        parameter),
                    typeof(object)),
                parameter
            )
            .Compile();

        ParameterExpression memoryValueParameter = Expression.Parameter(typeof(object));
        getValue = Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.Property(
                        Expression.Convert(memoryValueParameter, memoryValueType),
                        memoryValueProperty),
                    typeof(object)),
                memoryValueParameter)
            .Compile();

        ParameterExpression valueParameter = Expression.Parameter(typeof(object));
        setValue = Expression.Lambda<Action<object, object>>(
                Expression.Assign(
                    Expression.Property(
                        Expression.Convert(memoryValueParameter, memoryValueType),
                        memoryValueProperty),
                    Expression.Convert(valueParameter, propertyInfo.PropertyType)),
                memoryValueParameter,
                valueParameter)
            .Compile();
        return createMemoryValue;
    }

    public object? Value
    {
        get => _getValue();
        set => _setValue(value);
    }
}