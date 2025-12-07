using System.Linq.Expressions;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly record struct DataCollectionItem<TValue>(
    Expression<Func<TValue, long>> SelectNextExpression)
    where TValue : unmanaged, IDataValue<TValue>
{
    public Func<TValue, long> GetNext { get; } = SelectNextExpression.Compile();

    public Action<TValue, long> SetNext { get; } = Expression.Lambda<Action<TValue, long>>(
            Expression.Assign(SelectNextExpression, Expression.Parameter(typeof(long))),
            Expression.Parameter(typeof(long))
        )
        .Compile();

    public Func<TValue, long> SelectNext { get; } = SelectNextExpression.Compile();
}