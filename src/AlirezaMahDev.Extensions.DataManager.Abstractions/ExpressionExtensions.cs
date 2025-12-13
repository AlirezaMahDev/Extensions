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