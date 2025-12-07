using System.Linq.Expressions;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly record struct DataCollection<TValue, TItem>(
    Expression<Func<TValue, long>> SelectChildExpression,
    Expression<Func<TItem, long>> SelectNextExpression)
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
{
    public Func<TValue, long> GetChild { get; } = SelectChildExpression.Compile();

    public Action<TValue, long> SetChild { get; } = Expression.Lambda<Action<TValue, long>>(
            Expression.Assign(SelectChildExpression, Expression.Parameter(typeof(long))),
            Expression.Parameter(typeof(long))
        )
        .Compile();

    public DataCollectionItem<TItem> ItemWrap { get; } = new(SelectNextExpression);
}