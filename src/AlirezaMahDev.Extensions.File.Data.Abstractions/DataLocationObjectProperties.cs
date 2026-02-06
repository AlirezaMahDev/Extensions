using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public readonly record struct DataLocationObjectProperties : IEnumerable<DataLocationObjectProperty>
{
    private readonly DataLocationObjectProperty[] _properties = null!;

    private DataLocationObjectProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
    {
        var properties = type
            .GetProperties(BindingFlags.GetProperty |
                           BindingFlags.SetProperty |
                           BindingFlags.Public |
                           BindingFlags.Instance)
            .Where(x => x.SetMethod is not null &&
                        x.GetMethod is not null &&
                        x.GetCustomAttribute<NotMappedAttribute>() is null)
            .ToArray();
        PropertyInfos = [.. properties.Where(x => x.PropertyType.IsUnmanaged)];
        OtherPropertyInfos = [.. properties.Where(x => !x.PropertyType.IsUnmanaged)];
    }

    public DataLocationObjectProperties(Type type, Func<string, DataBlockMemory> func) : this(type)
    {
        _properties =
        [
            .. PropertyInfos
                .Select(x =>
                    new DataLocationObjectProperty(
                        func(x.Name),
                        x
                    )
                )
        ];
    }

    public DataLocationObjectProperties(Type type, IDataLocation location) : this(type)
    {
        _properties =
        [
            .. PropertyInfos
                .Select(x =>
                    new DataLocationObjectProperty(
                        location.GetOrAdd(x.Name),
                        x
                    )
                )
        ];
    }

    public PropertyInfo[] PropertyInfos { get; }
    public PropertyInfo[] OtherPropertyInfos { get; }

    public IEnumerator<DataLocationObjectProperty> GetEnumerator()
    {
        return _properties.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}