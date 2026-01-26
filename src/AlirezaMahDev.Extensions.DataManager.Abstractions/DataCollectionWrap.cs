using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
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