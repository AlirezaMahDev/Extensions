using System.Linq.Expressions;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class ExpressionExtensions
{
    extension<TSource, TValue>(Expression<SelectValueFunc<TSource, TValue>> selector)
    {
        public SetValueAction<TSource, TValue> BuildSetter()
        {
            var sourceParameter = Expression.Parameter(typeof(TSource).MakeByRefType(), "source");
            var valueParameter = Expression.Parameter(typeof(TValue), "value");

            if (selector.Body is not MemberExpression memberExpression)
                throw new ArgumentException(
                    "SelectChildExpression must be a direct member access like x => x.PropertyOrField",
                    nameof(selector));

            var target = Expression.MakeMemberAccess(sourceParameter, memberExpression.Member);
            var assign = Expression.Assign(target, valueParameter);
            return Expression.Lambda<SetValueAction<TSource, TValue>>(assign, sourceParameter, valueParameter)
                .Compile();
        }
    }
}

public readonly struct DataCollection<TValue, TItem>(
    Expression<SelectValueFunc<TValue, long>> selectChildExpression,
    Expression<SelectValueFunc<TItem, long>> selectNextExpression)
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
{
    public SelectValueFunc<TValue, long> GetChild { get; } = selectChildExpression.Compile();

    public SetValueAction<TValue, long> SetChild { get; } = selectChildExpression.BuildSetter();
    public DataCollectionItem<TItem> ItemWrap { get; } = new(selectNextExpression);
}