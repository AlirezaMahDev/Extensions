using System.Collections;

using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public readonly struct StackItems<TEntity>(IStackItems items) : IDataCollection<StackItem<TEntity>>
    where TEntity : unmanaged
{
    public IStackItems Items { get; } = items;

    public int Count => Items.Count;

    public StackItem<TEntity> Add()
    {
        return Items.Add().As<TEntity>();
    }

    public bool TryGet(int index, out StackItem<TEntity> item)
    {
        if (Items.TryGet(index, out var stackItem))
        {
            item = stackItem.As<TEntity>();
            return true;
        }

        item = default;
        return false;
    }

    public StackItem<TEntity> GetOrAdd(int index)
    {
        return Items.GetOrAdd(index).As<TEntity>();
    }

    public bool TryRemove(int index, out StackItem<TEntity> item)
    {
        if (Items.TryRemove(index, out var stackItem))
        {
            item = stackItem.As<TEntity>();
            return true;
        }

        item = default;
        return false;
    }

    public IEnumerator<StackItem<TEntity>> GetEnumerator()
    {
        return Items.Count.ToIdRange().Select(GetOrAdd).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}