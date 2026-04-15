namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefEnumerable<TSelf, T, TLockRefEnumerator> : ILockRefEnumerableCore<TLockRefEnumerator>
    where TSelf : ILockRefEnumerable<TSelf, T, TLockRefEnumerator>, allows ref struct
    where TLockRefEnumerator : ILockRefEnumerator<TLockRefEnumerator, T>, allows ref struct;