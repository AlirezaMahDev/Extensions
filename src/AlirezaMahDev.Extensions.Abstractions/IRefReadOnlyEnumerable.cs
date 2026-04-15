namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefReadOnlyEnumerable<TSelf, T, TRefReadOnlyEnumerator> : IRefEnumerableCore<TRefReadOnlyEnumerator>
    where TSelf : IRefReadOnlyEnumerable<TSelf, T, TRefReadOnlyEnumerator>, allows ref struct
    where TRefReadOnlyEnumerator : IRefReadOnlyEnumerator<TRefReadOnlyEnumerator, T>, allows ref struct;