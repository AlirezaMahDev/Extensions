using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

[MustDisposeResource]
public readonly record struct DataLocationObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity> : IDataBlockAccessorSave, IDisposable
    where TEntity : class
{
    public DataLocationObject(Func<string, DataBlockMemory> func)
    {
        ObjectProperties = new(typeof(TEntity), func);
        Proxy = new(ObjectProperties);
    }

    public DataLocationObject(IDataLocation location)
    {
        ObjectProperties = new(typeof(TEntity), location);
        Proxy = new(location, ObjectProperties);
    }

    private DataLocationObjectProperties ObjectProperties { get; }
    private DataLocationObjectProxy<TEntity> Proxy { get; }
    public TEntity Entity => Proxy.Entity;

    public void Save() => Proxy.Save();

    public void Dispose() => Save();
}