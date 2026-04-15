namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefEnumerable<TSelf, T, TRefEnumerator> : IRefEnumerableCore<TRefEnumerator>
    where TSelf : IRefEnumerable<TSelf, T, TRefEnumerator>, allows ref struct
    where TRefEnumerator : IRefEnumerator<TRefEnumerator, T>, allows ref struct;