using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataCollection<TItem> : IReadOnlyCollection<TItem>
{
    bool TryGet(int index, [MaybeNullWhen(false)] out TItem item);
    TItem GetOrAdd(int index);
    bool TryRemove(int index, [MaybeNullWhen(false)] out TItem item);
    TItem Add();
}

public interface IDataCollection<in TValue, TItem> : IReadOnlyCollection<TItem>
{
    bool TryGet(int index, [MaybeNullWhen(false)] out TItem value);
    TItem GetOrAdd(int index, TValue size);
    bool TryRemove(int index, [MaybeNullWhen(false)] out TItem item);
    TItem Add(TValue value);
}