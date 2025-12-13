using System.Linq.Expressions;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly struct DataCollectionItem<TValue>(
    Expression<SelectValueFunc<TValue, long>> selectNextExpression)
    where TValue : unmanaged, IDataValue<TValue>
{
    public SelectValueFunc<TValue, long> GetNext { get; } = selectNextExpression.Compile();

    public SetValueAction<TValue, long> SetNext { get; } = selectNextExpression.BuildSetter();

    public SelectValueFunc<TValue, long> SelectNext { get; } = selectNextExpression.Compile();
}