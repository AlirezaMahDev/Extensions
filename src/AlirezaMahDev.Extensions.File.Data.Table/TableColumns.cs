using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableColumns : ITableColumns
{
    private readonly TableColumnFactory _columnFactory;
    private readonly ConcurrentDictionary<String64, Lazy<ITableColumn>> _cache = [];

    public TableColumns(TableAccess access, TableColumnFactory columnFactory)
    {
        _columnFactory = columnFactory;
        Access = access;
        Location = Access.Location.GetOrAdd(".columns");
    }

    public ITableAccess Access { get; }
    public IDataLocation Location { get; }

    public ref String64 RefKey => ref Location.RefKey;

    public bool TryGet(String64 key, [MaybeNullWhen(false)] out ITableColumn @object)
    {
        if (_cache.TryGetValue(key, out var lazy))
        {
            @object = lazy.Value;
            return true;
        }

        @object = this.FirstOrDefault(x => x.RefKey == key);
        if (@object is not null)
        {
            _cache.TryAdd(key, new(@object));
            return true;
        }

        return false;
    }

    public ITableColumn GetOrAdd(String64 key)
    {
        var columns = this;
        return _cache.GetOrAdd(key,
                static (key, columns) =>
                    new(() => columns._columnFactory.GetOrCreate(new(columns, key)),
                        LazyThreadSafetyMode.ExecutionAndPublication),
                columns)
            .Value;
    }

    public bool TryRemove(String64 key, [MaybeNullWhen(false)] out ITableColumn @object)
    {
        if (!TryGet(key, out @object))
        {
            return false;
        }

        @object.RefValue.DeleteAt = DateTimeOffset.Now;
        return true;
    }

    public virtual IEnumerator<ITableColumn> GetEnumerator()
    {
        return Location.Select(x => GetOrAdd(x.RefKey)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Save()
    {
        Location.Save();
        foreach (var keyValuePair in _cache)
        {
            keyValuePair.Value.Value.Save();
        }
    }
}