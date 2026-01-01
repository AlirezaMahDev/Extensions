using System.Linq.Expressions;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly struct DataCollectionWrap<TValue, TItem>(
    Expression<SelectValueFunc<TValue, long>> selectChildExpression,
    Expression<SelectValueFunc<TItem, long>> selectNextExpression)
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
{
    public SelectValueFunc<TValue, long> GetChild { get; } = selectChildExpression.Compile();

    public SetValueAction<TValue, long> SetChild { get; } = selectChildExpression.BuildSetter();
    public DataCollectionItemWrap<TItem> ItemWrap { get; } = new(selectNextExpression);
}