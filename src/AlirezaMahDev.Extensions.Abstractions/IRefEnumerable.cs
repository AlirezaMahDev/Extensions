namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefEnumerable<TSelf, T, TEnumerator> : IRefReadOnlyEnumerable<TSelf, T, RefEnumeratorToRefReadOnlyEnumerator<TEnumerator, T>>
    where TSelf : IRefEnumerable<TSelf, T, TEnumerator>, allows ref struct
    where TEnumerator : IRefEnumerator<TEnumerator, T>, allows ref struct;