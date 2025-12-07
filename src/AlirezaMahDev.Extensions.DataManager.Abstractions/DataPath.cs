using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct DataPath(String64 Key,int Size, long Next, long Child, long Data,  long Index)
    : IDataDictionaryTree<DataPath, String64>, IDataValueDefault<DataPath>, IDataStorage<DataPath>, IDataIndex<DataPath>
{
    public static DataPath Default { get; } = new(default, -1, -1L, -1L, -1L,-1L);
}

public interface IDataIndex<TSelf> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
{
    long Index { get; set; }
}

public interface IDataIndexer<TSelf> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
{
    
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct DataIndex(String64 Key);

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct DataIndexStorage(String64 Key);

public class DataQueryProvider : IQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        throw new NotImplementedException();
    }

    public object? Execute(Expression expression)
    {
        throw new NotImplementedException();
    }

    public TResult Execute<TResult>(Expression expression)
    {
        throw new NotImplementedException();
    }
}