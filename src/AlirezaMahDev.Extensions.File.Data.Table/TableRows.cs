using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Stack;
using AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableRows : ITableRows
{
    private readonly TableRowFactory _rowFactory;
    private readonly ConcurrentDictionary<int, Lazy<ITableRow>> _cache = [];

    public TableRows(TableAccess access, TableRowFactory rowFactory)
    {
        _rowFactory = rowFactory;
        Table = access;
        LocationValue = access.Location.GetOrAdd<TableRowsValue>(".rows");
        Stack = LocationValue.Location.AsStack();
    }

    public ITableAccess Table { get; }
    public DataLocation<TableRowsValue> LocationValue { get; }
    public IStackAccess Stack { get; }

    public ref String64 RefKey => ref LocationValue.RefKey;
    public ref TableRowsValue RefValue => ref LocationValue.RefValue;
    public int Count => RefValue.Count;

    public bool TryGet(int index, [MaybeNullWhen(false)] out ITableRow item)
    {
        if (_cache.TryGetValue(index, out var lazy))
        {
            item = lazy.Value;
            return true;
        }

        item = this.FirstOrDefault(x => x.Index == index);
        if (item is not null)
        {
            _cache.TryAdd(index, new(item));
            return true;
        }

        return false;
    }

    public ITableRow GetOrAdd(int index)
    {
        return _cache.GetOrAdd(index,
                static (key, rows) =>
                    new(() =>
                            rows._rowFactory.GetOrCreate(new(rows, key)),
                        LazyThreadSafetyMode.ExecutionAndPublication),
                this)
            .Value;
    }

    public ITableRow Add()
    {
        return GetOrAdd(Interlocked.Increment(ref RefValue.Count));
    }

    public bool TryRemove(int index, [MaybeNullWhen(false)] out ITableRow row)
    {
        if (!TryGet(index, out row))
        {
            return false;
        }

        row.Value.DeleteAt = DateTimeOffset.Now;
        return true;
    }

    public virtual IEnumerator<ITableRow> GetEnumerator()
    {
        return RefValue.Count.ToIdRange().Select(GetOrAdd).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Save()
    {
        LocationValue.Save();
        Stack.Save();
    }
}