namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefReadOnlyEnumerable<TSelf, T, out TEnumerator>
    where TSelf : IRefReadOnlyEnumerable<TSelf, T, TEnumerator>, allows ref struct
    where TEnumerator : IRefReadOnlyEnumerator<TEnumerator, T>, allows ref struct
{
    TEnumerator GetEnumerator();
}