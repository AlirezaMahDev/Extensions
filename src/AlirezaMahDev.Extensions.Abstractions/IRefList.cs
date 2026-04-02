namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefList<TSelf, T, TEnumerator> : IRefBlock<TSelf, T, TEnumerator>
    where TSelf : IRefList<TSelf, T, TEnumerator>, allows ref struct
    where TEnumerator : IRefEnumerator<TEnumerator, T>, allows ref struct
{
    bool Add(in T value);
    bool Insert(in int index, in T value);
    bool Remove(in int index);
}