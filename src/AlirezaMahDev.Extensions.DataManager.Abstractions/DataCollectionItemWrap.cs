using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct DataCollectionItemWrap<TValue>(
    Expression<SelectValueFunc<TValue, DataOffset>> selectNextExpression)
    where TValue : unmanaged, IDataValue<TValue>
{
    public SelectValueFunc<TValue, DataOffset> GetNext { get; } = selectNextExpression.Compile();

    public SetValueAction<TValue, DataOffset> SetNext { get; } = selectNextExpression.BuildSetter();

    public SelectValueFunc<TValue, DataOffset> SelectNext { get; } = selectNextExpression.Compile();
}