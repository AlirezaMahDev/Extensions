using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[RequiresUnreferencedCode("This functionality is not compatible with trimming. Use 'MethodFriendlyToTrimming' instead")]
public class DataLocationObjectProxy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>
    where TEntity : class
{
    private readonly Action _saveAction = () => { };
    public TEntity Entity { get; } = Activator.CreateInstance<TEntity>();

    [RequiresUnreferencedCode(
        "This functionality is not compatible with trimming. Use 'MethodFriendlyToTrimming' instead")]
    public DataLocationObjectProxy(DataLocationObjectProperties properties)
    {
        foreach (DataLocationObjectProperty property in properties)
        {
            property.PropertyInfo.SetValue(Entity, property.Value);
            _saveAction += () => property.Value = property.PropertyInfo.GetValue(Entity);
        }

        if (properties.OtherPropertyInfos.Length != 0)
        {
            throw new NotSupportedException("in non location object other properties not supported");
        }
    }

    [RequiresUnreferencedCode(
        "This functionality is not compatible with trimming. Use 'MethodFriendlyToTrimming' instead")]
    public DataLocationObjectProxy(IDataLocation dataLocation, DataLocationObjectProperties properties)
    {
        foreach (DataLocationObjectProperty property in properties)
        {
            property.PropertyInfo.SetValue(Entity, property.Value);
            _saveAction += () => property.Value = property.PropertyInfo.GetValue(Entity);
        }

        foreach (PropertyInfo propertyInfo in properties.OtherPropertyInfos)
        {
            IDataLocation propertyLocation = dataLocation.GetOrAdd(propertyInfo.Name);
            Type type = typeof(DataLocationObjectProxy<>).MakeGenericType(propertyInfo.PropertyType);
            object? instance = Activator.CreateInstance(
                type,
                propertyLocation,
                new DataLocationObjectProperties(propertyInfo.PropertyType, propertyLocation)
            );
            PropertyInfo entityPropertyInfo = type.GetProperty(nameof(Entity))!;
            MethodInfo entitySaveMethod = type.GetMethod(nameof(Save))!;
            propertyInfo.SetValue(Entity, entityPropertyInfo.GetValue(instance));
            _saveAction += () => entitySaveMethod.Invoke(instance, []);
        }
    }

    public void Save()
    {
        _saveAction();
    }
}