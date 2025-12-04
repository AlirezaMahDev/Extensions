using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Stack;

internal class StackItems(StackAccess stackAccess, StackItemFactory itemFactory) : IStackItems
{
    private readonly StackItemFactory _itemFactory = itemFactory;
    private readonly ConcurrentDictionary<int, Lazy<IStackItem>> _cache = [];

    public IStackAccess StackAccess { get; } = stackAccess;

    public DataLocation<StackItemsValue> LocationValue { get; } =
        stackAccess.LocationValue.Location.GetOrAdd<StackItemsValue>("items");

    public IDataLocation Location => LocationValue.Location;

    public ref String64 RefKey => ref LocationValue.RefKey;
    public ref StackItemsValue RefValue => ref LocationValue.RefValue;

    public int Count => RefValue.Count;

    public bool TryGet(int index, [MaybeNullWhen(false)] out IStackItem item)
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

    public IStackItem GetOrAdd(int index)
    {
        return _cache.GetOrAdd(index,
                static (key, items) =>
                    new(() =>
                            items._itemFactory.GetOrCreate(new(items, key)),
                        LazyThreadSafetyMode.ExecutionAndPublication),
                this)
            .Value;
    }

    public IStackItem Add()
    {
        return GetOrAdd(Interlocked.Increment(ref RefValue.Count));
    }

    public bool TryRemove(int index, [MaybeNullWhen(false)] out IStackItem item)
    {
        if (!TryGet(index, out item))
        {
            return false;
        }

        item.Remove();
        return true;
    }

    public virtual IEnumerator<IStackItem> GetEnumerator()
    {
        return RefValue.Count.ToIdRange().Select(GetOrAdd).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Save()
    {
        LocationValue.Location.Save();
        foreach (var location in LocationValue.Location)
        {
            location.Save();
        }
    }
}