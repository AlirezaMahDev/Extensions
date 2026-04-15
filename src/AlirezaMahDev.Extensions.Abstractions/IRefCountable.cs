namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefCountable<TSelf, T, TRefEnumerator> : IRefEnumerable<TSelf, T, TRefEnumerator>, IRefLength
    where TSelf : IRefCountable<TSelf, T, TRefEnumerator>, allows ref struct
    where TRefEnumerator : IRefEnumerator<TRefEnumerator, T>, allows ref struct;