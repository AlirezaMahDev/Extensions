using System.Collections;

using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public readonly struct TableRows<TEntity>(ITableRows rows) : IDataCollection<TableRow<TEntity>>
    where TEntity : class
{
    public ITableRows Rows { get; } = rows;
    public int Count => Rows.Count;

    public bool TryGet(int index, out TableRow<TEntity> item)
    {
        if (Rows.TryGet(index, out var row))
        {
            item = new(row);
            return true;
        }

        item = default;
        return false;
    }

    public TableRow<TEntity> GetOrAdd(int index)
    {
        return new(Rows.GetOrAdd(index));
    }

    public bool TryRemove(int index, out TableRow<TEntity> item)
    {
        if (Rows.TryRemove(index, out var row))
        {
            item = new(row);
            return true;
        }

        item = default;
        return false;
    }

    public TableRow<TEntity> Add()
    {
        throw new NotImplementedException();
    }


    public IEnumerator<TableRow<TEntity>> GetEnumerator()
    {
        return Rows.Select(x => x.As<TEntity>()).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}