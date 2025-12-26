using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public readonly struct TableRow<TEntity>(ITableRow row) : IDisposable, IDataBlockAccessorSave
    where TEntity : class
{
    private readonly DataLocationObject<TEntity> _dataLocationObject =
        new(s => row.GetOrAdd(s).BlockMemory);

    public ITableRow Row { get; } = row;
    public int Index { get; } = row.Index;

    public TEntity Entity => _dataLocationObject.Entity;

    public void Save() => _dataLocationObject.Save();
    public void Dispose() => Save();
}