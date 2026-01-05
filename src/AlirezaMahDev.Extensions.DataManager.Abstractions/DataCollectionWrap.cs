using System.Linq.Expressions;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly struct DataCollectionWrap<TValue, TItem>(
    Expression<SelectValueFunc<TValue, DataOffset>> selectChildExpression,
    Expression<SelectValueFunc<TItem, DataOffset>> selectNextExpression)
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
{
    public SelectValueFunc<TValue, DataOffset> GetChild { get; } = selectChildExpression.Compile();

    public SetValueAction<TValue, DataOffset> SetChild { get; } = selectChildExpression.BuildSetter();
    public DataCollectionItemWrap<TItem> ItemWrap { get; } = new(selectNextExpression);
}