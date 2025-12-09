using System.Linq.Expressions;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate TResult SelectValueFunc<in T, out TResult>(T arg)
    where T : allows ref struct
    where TResult : allows ref struct;

public delegate void SetValueAction<TSource, in TValue>(ref TSource source, TValue value)
    where TSource : allows ref struct
    where TValue : allows ref struct;

public readonly struct DataCollectionItem<TValue>(
    Expression<SelectValueFunc<TValue, long>> selectNextExpression)
    where TValue : unmanaged, IDataValue<TValue>
{
    public SelectValueFunc<TValue, long> GetNext { get; } = selectNextExpression.Compile();

    public SetValueAction<TValue, long> SetNext { get; } = selectNextExpression.BuildSetter();

    public SelectValueFunc<TValue, long> SelectNext { get; } = selectNextExpression.Compile();
}